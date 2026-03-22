# Shayou 通信协议与前端消息总线方案

## 1. 目标

当前项目的通信主要围绕游戏内流程，后端向前端发送事件，前端再回一个字符串输入。这种方式能支撑最基础的回合交互，但不适合继续扩展房间、准备、开始游戏、重连、多前端复用这些需求。

本方案的目标是：

- 保持协议尽量简单
- 保留前端真正需要的 `key`
- 让 `Host.Local` 和未来的 `UI.Godot` 复用同一套前端通信运行时
- 为后续房间逻辑和完整前后端通信留出扩展空间

## 2. 现状问题

当前项目的协议层有：

- `EventPacket`
- `EventResponsePacket`
- `BroadcastPacket`
- `PacketBatch`

但实际链路是：

- 后端 -> 前端：结构化包
- 前端 -> 后端：裸 `string input`

这会带来几个问题：

1. 前后端不对称  
后端有协议，前端没有正式协议。

2. `EventPacket` 和 `BroadcastPacket` 语义重叠  
它们本质上都是后端主动推给前端的消息。

3. 前端逻辑难复用  
`Host.Local` 会写一份消息分发，以后 Godot 还要再写一份。

4. 游戏外逻辑无法自然表达  
比如：
   - 创建房间
   - 加入房间
   - 准备
   - 开始游戏
   - 重连

5. 当前输入请求链路过于特化  
只能处理“服务端发请求，客户端回字符串”。

## 3. 设计原则

这次方案按你的要求，保持最简，不引入多余字段。

### 3.1 只保留一个核心业务 key

协议里只保留一个 `Key`。

这个 `Key` 同时承担：

- 协议 key
- 业务 key
- Valid key
- 前端反馈逻辑 key

也就是说，前端收到一个包后，直接拿 `Key` 去找对应的反馈逻辑和 Valid。

示例：

- `room.Create`
- `room.Ready`
- `room.StartGame`
- `game.PlayPhase`
- `game.DiscardPhase`
- `game.ChooseTarget`

### 3.2 不再额外拆 `protocolKey`

不引入额外的：

- `protocolKey`
- `businessKey`
- 独立的 `validatorKey`

原因很简单：

- 这些信息在当前项目里不是必须的
- 前端真正依赖的就是一个统一的 `Key`
- `Valid` 已经承载了大部分交互约束和反馈逻辑

### 3.3 `payload` 不是主设计

当前方案默认不依赖 `payload`。

原因：

- 你已经明确说明，大部分交互信息都在 `Valid` 里
- 现在没必要为了“可能未来要用”而把协议复杂化

如果以后真的有少量扩展需求，可以再补一个可选 `Data` 字段，但不作为主通信模型。

### 3.4 只保留必要的消息语义

建议保留这几类消息：

- `command`
- `response`
- `event`
- `snapshot`
- `error`

这里的区别只放在 `Kind` 上，具体业务靠 `Key` 区分。

## 4. 推荐协议模型

## 4.1 最小可行模型

推荐在 `Protocol` 中逐步收敛到下面这个模型：

```csharp
namespace Shayou.Protocol.Messages;

public abstract record PacketEnvelope
{
    public required string Kind { get; init; }
    public required string Key { get; init; }
}

public record CommandPacket : PacketEnvelope;

public record ResponsePacket : PacketEnvelope
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
}

public record EventPacket : PacketEnvelope;

public record SnapshotPacket : PacketEnvelope;

public record ErrorPacket : PacketEnvelope
{
    public required string ErrorMessage { get; init; }
}
```

这套模型的核心就是：

- `Kind` 决定消息属于哪种协议语义
- `Key` 决定具体业务和前端反馈逻辑

## 4.2 `Kind` 的职责

### `command`

前端主动发给后端的动作。

示例：

- `room.Create`
- `room.Join`
- `room.Ready`
- `room.StartGame`
- `game.Pass`
- `game.Confirm`
- `game.Cancel`

### `response`

后端对某个命令的结果回应。

示例：

- `room.Create`
- `room.Join`
- `room.StartGame`

是否成功由 `Success` 表示。

### `event`

后端主动推给前端的事实通知。

示例：

- `room.MemberJoined`
- `room.MemberLeft`
- `game.PlayPhase`
- `game.DiscardPhase`
- `game.ChooseTarget`

### `snapshot`

后端发给前端的完整状态同步。

示例：

- `room.State`
- `game.State`

### `error`

系统级错误或无法归类到正常响应的错误消息。

## 4.3 `Key` 的职责

`Key` 是这套协议里最重要的字段。

前端收到消息后，直接根据 `Key`：

- 查找 Valid
- 查找反馈逻辑
- 决定显示哪个界面
- 决定当前可用的交互模式

例如：

```json
{
  "kind": "event",
  "key": "game.PlayPhase"
}
```

前端就可以直接：

- 根据 `game.PlayPhase` 找到对应 Valid
- 根据 `game.PlayPhase` 进入 PlayPhase 的反馈逻辑

这正是你要的效果。

## 5. `BroadcastPacket` 是否保留

建议不保留独立的 `BroadcastPacket`。

原因：

- 广播只是发送方式，不是协议类型
- 对前端来说，真正重要的是：
  - 这是 `event`
  - 还是 `snapshot`
  - 它的 `Key` 是什么

所以建议：

- 普通通知统一用 `EventPacket`
- 状态同步统一用 `SnapshotPacket`
- 不再区分独立的 `BroadcastPacket`

## 6. 前端向后端的非游戏内包设计

前端向后端的非游戏内逻辑统一走 `CommandPacket`。

建议的 `Key` 例如：

- `room.Create`
- `room.Join`
- `room.Leave`
- `room.Ready`
- `room.CancelReady`
- `room.StartGame`
- `room.SelectMode`

示例：

```json
{
  "kind": "command",
  "key": "room.Create"
}
```

对应响应：

```json
{
  "kind": "response",
  "key": "room.Create",
  "success": true
}
```

当前阶段这样已经足够。

## 7. 游戏内包如何设计

游戏内也继续走同一套协议，不再额外搞一套特殊机制。

示例：

### 后端通知进入出牌阶段

```json
{
  "kind": "event",
  "key": "game.PlayPhase"
}
```

### 前端提交“过”

```json
{
  "kind": "command",
  "key": "game.Pass"
}
```

### 前端点击确认

```json
{
  "kind": "command",
  "key": "game.Confirm"
}
```

### 前端点击取消

```json
{
  "kind": "command",
  "key": "game.Cancel"
}
```

这套设计和你现在的 Valid 思路是相容的：

- 事件到了，前端按 `Key` 找反馈逻辑
- 具体哪些对象能选、能不能确认，交给 Valid

## 8. 前端消息总线必须封装

这个结论不变，而且越早做越好。

如果不封装，以后每个前端都要自己写一套：

- 收包
- 拆 `PacketBatch`
- 按 `Kind + Key` 分发
- 对接 Valid
- 更新前端状态

这会导致：

- `Host.Local` 写一份
- `UI.Godot` 再写一份
- 将来如果有新前端还要继续写

这没有必要。

## 9. 建议新增 `ClientRuntime`

建议新增一个共享项目：

- `ClientRuntime`

依赖关系建议如下：

```text
Protocol
  └─ ClientRuntime
       ├─ Host.Local
       └─ UI.Godot
```

它的职责是：

- 统一收发协议包
- 统一拆 `PacketBatch`
- 统一按 `Kind + Key` 分发
- 统一对接 Valid
- 统一维护前端状态
- 对外提供简单 API

## 10. `ClientRuntime` 的模块划分

## 10.1 Transport

统一客户端传输接口：

```csharp
public interface IGameClientTransport
{
    void SendPacket(PacketEnvelope packet);
    PacketEnvelope WaitForPacket();
}
```

说明：

- 双向都只收发结构化包
- 不再保留 `SendInput(string)` 这种特化接口

## 10.2 Message Bus / Dispatcher

负责：

- 接收消息
- 处理 `PacketBatch`
- 按 `Kind + Key` 分发

推荐接口：

```csharp
public interface IGameClientMessageBus
{
    void Publish(PacketEnvelope packet);
    IDisposable Subscribe(string kind, string key, Action<PacketEnvelope> handler);
}
```

前端逻辑可以直接订阅：

- `event + game.PlayPhase`
- `event + game.DiscardPhase`
- `snapshot + room.State`

## 10.3 Store

负责维护共享状态，避免每个前端自己拼状态。

建议至少有：

- `SessionState`
- `RoomState`
- `GameState`

## 10.4 API / Facade

给 UI 层用，不让 UI 自己拼协议包。

例如：

```csharp
public interface IGameClientApi
{
    void CreateRoom();
    void JoinRoom();
    void Ready();
    void CancelReady();
    void StartGame();
    void Pass();
    void Confirm();
    void Cancel();
}
```

## 11. Host.Local 和 UI 的职责边界

### `ClientRuntime` 负责

- 协议解释
- 消息分发
- Valid 对接
- 状态同步

### `Host.Local` 负责

- 控制台输入输出
- 调用 `ClientRuntime` 提供的 API
- 打印状态变化

### `UI.Godot` 负责

- 具体界面展示
- 按钮绑定
- 根据状态刷新场景
- 调用 `ClientRuntime` API

结论：

- `Host.Local` 不应该继续直接处理协议细节
- 它应该只是 `ClientRuntime` 的一个壳

## 12. 推荐目录结构

```text
Shayou
├─ Protocol
│  ├─ Messages
│  │  ├─ PacketEnvelope.cs
│  │  ├─ CommandPacket.cs
│  │  ├─ ResponsePacket.cs
│  │  ├─ EventPacket.cs
│  │  ├─ SnapshotPacket.cs
│  │  └─ ErrorPacket.cs
│  ├─ Serialization
│  └─ Transport
├─ ClientRuntime
│  ├─ Transport
│  ├─ Messaging
│  ├─ State
│  ├─ Api
│  └─ Runtime
├─ Engine
├─ Gameplay
├─ Host
│  └─ Local
└─ UI
   └─ Godot
```

## 13. 服务端改造建议

## 13.1 传输接口统一化

当前传输接口是一边包、一边字符串，建议改成双向统一包。

推荐统一为：

```csharp
public interface IServerConnection
{
    void SendPacket(PacketEnvelope packet);
    PacketEnvelope WaitForPacket();
}

public interface IClientConnection
{
    void SendPacket(PacketEnvelope packet);
    PacketEnvelope WaitForPacket();
}
```

## 13.2 输入链路改造

当前 `InputManager` 是：

- 后端发输入请求
- 前端回字符串

建议改成：

- 后端发 `event + game.PlayPhase`
- 前端发 `command + game.Pass` / `game.Confirm` / `game.Cancel`

也就是说，输入链路不再特殊化，而是并入统一协议。

## 13.3 快照同步

建议补充完整状态快照：

- `snapshot + room.State`
- `snapshot + game.State`

这样可以支持：

- 新玩家进入
- 前端初始化
- 断线重连
- 状态纠偏

## 14. 分阶段落地计划

## Phase 1：简化协议

目标：

- 收敛协议模型
- 保留 `Kind + Key`
- 去掉多余概念

任务：

- 新增 `CommandPacket`
- 新增 `ResponsePacket`
- 新增 `SnapshotPacket`
- 新增 `ErrorPacket`
- 收敛 `PacketEnvelope`
- 保留唯一业务字段 `Key`
- 逐步淘汰 `BroadcastPacket`

## Phase 2：统一前后端输入输出

目标：

- 去掉裸字符串输入

任务：

- 移除 `SendInput(string)`
- 移除 `WaitForInput()`
- 改成双向 `SendPacket / WaitForPacket`
- 输入行为统一走 `command`

## Phase 3：引入 `ClientRuntime`

目标：

- 让前端通信逻辑只写一份

任务：

- 新建 `ClientRuntime`
- 实现消息总线
- 实现状态仓库
- 实现 API
- `Host.Local` 接入
- `UI.Godot` 接入

## Phase 4：补齐房间域

目标：

- 支持正式的游戏外流程

任务：

- `room.Create`
- `room.Join`
- `room.Leave`
- `room.Ready`
- `room.StartGame`
- `room.State`

## 15. 最终结论

### 协议层

当前项目最适合的不是复杂协议，而是最简协议：

- `Kind`
- `Key`

其中：

- `Kind` 表示消息语义
- `Key` 直接就是协议 key、业务 key、Valid key、前端反馈逻辑 key

### `BroadcastPacket`

不建议继续保留为独立协议包。

### `payload`

当前不作为主设计。

### 前端消息总线

必须封装，建议抽成共享的 `ClientRuntime`。

### 多前端复用

`Host.Local`、`UI.Godot` 都应该依赖同一套前端运行时，而不是各写一套协议解释逻辑。

## 16. 下一步建议

建议按以下顺序推进：

1. 先重构 `Protocol`，把模型收敛到 `Kind + Key`
2. 再统一传输接口，去掉裸字符串输入
3. 新建 `ClientRuntime`
4. 最后补齐房间逻辑和快照同步

如果需要，下一步可以直接继续做代码落地：

1. 重构 `Protocol` 的消息类
2. 搭 `ClientRuntime` 的基础骨架
3. 把 `Host.Local` 改成基于 `ClientRuntime` 的实现
