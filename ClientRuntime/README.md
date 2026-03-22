# ClientRuntime 使用说明

`ClientRuntime` 是面向 UI 层的客户端 SDK，负责把底层收发包能力整理成四个稳定入口：

- `Api`：给 UI 主动发命令。
- `Bus`：给 UI 订阅服务端消息。
- `Session`：记录最近一条收到的消息。
- `Room` / `Game`：记录最近一条 `room.*` / `game.*` 相关消息。

它本身不关心你是本地联调、管道通信，还是网络连接；宿主只需要提供一个 `IGameClientTransport`。

## 对外接口

### 1. 初始化入口

```csharp
var runtime = new GameClientRuntime(transport);
```

- `transport` 需要实现 `IGameClientTransport`
- 如果你手里已经有 `IClientConnection`，可以直接包一层 `ClientConnectionTransport`

### 2. 发命令

`runtime.Api` 目前提供：

- `CreateRoom()`
- `JoinRoom()`
- `Ready()`
- `CancelReady()`
- `StartGame()`
- `Pass()`
- `Confirm()`
- `Cancel()`
- `SendCommand(string key)`

其中 `SendCommand` 是兜底入口。凡是 SDK 还没包成独立方法的协议，都可以直接发，比如：

```csharp
runtime.Api.SendCommand("room.Leave");
```

### 3. 收消息

`runtime.Bus.Subscribe(kind, key, handler)` 用来订阅消息：

- `kind` 精确匹配，例如 `PacketKinds.Event`
- `key` 为 `null` 时，表示订阅该 `kind` 下的全部消息
- 返回值是 `IDisposable`，UI 销毁时应主动 `Dispose()`

常见订阅方式：

```csharp
runtime.Bus.Subscribe(PacketKinds.Event, null, OnPacket);
runtime.Bus.Subscribe(PacketKinds.Response, null, OnPacket);
runtime.Bus.Subscribe(PacketKinds.Snapshot, null, OnPacket);
runtime.Bus.Subscribe(PacketKinds.Error, null, OnPacket);
```

### 4. 状态对象

`ClientRuntime` 只维护轻量状态，不做完整前端状态树。

- `runtime.Session.LastKind`：最近一条消息的 `Kind`
- `runtime.Session.LastKey`：最近一条消息的 `Key`
- `runtime.Room.LastKey`：最近一条 `room.*` 消息的 `Key`
- `runtime.Game.LastKey`：最近一条 `game.*` 消息的 `Key`

如果 UI 需要更完整的展示状态，应该在订阅回调里基于消息继续组装自己的 ViewModel / Store。

## 接入步骤

1. 宿主实现或提供一个 `IGameClientTransport`
2. 创建 `GameClientRuntime`
3. 先注册 `Bus` 订阅
4. 调用 `Start()` 启动接收循环
5. 通过 `Api` 响应按钮点击、快捷键或 UI 操作

推荐顺序如下：

```csharp
var runtime = new GameClientRuntime(transport);
RegisterSubscriptions(runtime);
runtime.Start();
```

## 线程说明

`Start()` 会拉起一个后台线程持续调用 `WaitForPacket()`。消息到达后：

- 先更新 `Session` / `Room` / `Game`
- 再同步触发 `Bus` 里的订阅回调

这意味着 `handler` 默认运行在接收线程，不在 UI 主线程。  
如果你接的是 Godot、WPF、WinForms 或其他 UI 框架，回调里要自行切回 UI 线程后再改界面状态。

另外，当前版本只有 `Start()`，没有 `Stop()`；通常把它当成和页面 / 客户端同生命周期对象来使用。

## 最小例程

下面这个例程是对 [Host/Local/Program.cs](../Host/Local/Program.cs) 用法的 UI 化缩减版。它展示了 UI 层最常见的接法：初始化、订阅、启动、发命令、清理订阅。

```csharp
using Shayou.ClientRuntime.Runtime;
using Shayou.ClientRuntime.Transport;
using Shayou.Protocol.Messages;
using Shayou.Protocol.Transport;

public sealed class LobbyViewModel : IDisposable
{
    private readonly GameClientRuntime runtime;
    private readonly List<IDisposable> subscriptions = new();

    public LobbyViewModel(IClientConnection clientConnection)
    {
        runtime = new GameClientRuntime(
            new ClientConnectionTransport(clientConnection));

        subscriptions.Add(runtime.Bus.Subscribe(PacketKinds.Event, null, OnPacket));
        subscriptions.Add(runtime.Bus.Subscribe(PacketKinds.Response, null, OnPacket));
        subscriptions.Add(runtime.Bus.Subscribe(PacketKinds.Snapshot, null, OnPacket));
        subscriptions.Add(runtime.Bus.Subscribe(PacketKinds.Error, null, OnPacket));

        runtime.Start();
    }

    public void CreateRoom()
    {
        runtime.Api.CreateRoom();
    }

    public void JoinRoom()
    {
        runtime.Api.JoinRoom();
    }

    public void Ready()
    {
        runtime.Api.Ready();
    }

    public void Pass()
    {
        runtime.Api.Pass();
    }

    public void Confirm()
    {
        runtime.Api.Confirm();
    }

    public void Cancel()
    {
        runtime.Api.Cancel();
    }

    public void LeaveRoom()
    {
        runtime.Api.SendCommand("room.Leave");
    }

    private void OnPacket(PacketEnvelope packet)
    {
        // Marshal to the UI thread before touching controls or bound state.
        Console.WriteLine($"{packet.Kind} {packet.Key}");
        Console.WriteLine($"session = {runtime.Session.LastKind}/{runtime.Session.LastKey}");
        Console.WriteLine($"room = {runtime.Room.LastKey}");
        Console.WriteLine($"game = {runtime.Game.LastKey}");
    }

    public void Dispose()
    {
        foreach (IDisposable subscription in subscriptions)
        {
            subscription.Dispose();
        }
    }
}
```

## 宿主侧最小要求

UI 宿主只需要满足下面这层抽象：

```csharp
public interface IGameClientTransport
{
    void SendPacket(PacketEnvelope packet);
    PacketEnvelope WaitForPacket();
}
```

也就是说，网络、命名管道、WebSocket、本地回环，都可以接进来；只要最后能适配成这两个方法，`ClientRuntime` 就能复用。

## 参考实现

- 接线参考：[Host/Local/Program.cs](../Host/Local/Program.cs)
- `IClientConnection` 适配层：[ClientRuntime/Transport/ClientConnectionTransport.cs](./Transport/ClientConnectionTransport.cs)
- 消息分发实现：[ClientRuntime/Messaging/GameClientMessageBus.cs](./Messaging/GameClientMessageBus.cs)
- 运行时入口：[ClientRuntime/Runtime/GameClientRuntime.cs](./Runtime/GameClientRuntime.cs)
