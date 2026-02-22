# Bang! 游戏引擎项目结构

## 项目概述

这是一个模块化的卡牌游戏引擎，支持《Bang!》及其衍生游戏（如《三国杀》、《国战》等）。项目采用分层架构设计，实现了核心逻辑与具体规则的解耦。

## 目录结构

```
├── Common/                         // 跨模块原子定义（不依赖其他任何模块）
│   ├── Interfaces/                 // 基础契约：IEntity, IEffect, ITargetable, IAction, IComponent
│   └── Models/                     // 基础结构：ActionResult, GameConfig
│
├── Core/                           // 核心引擎大脑（纯逻辑，无UI，无具体规则）
│   ├── Domain/                     // 实体层
│   │   ├── Entities/               // Player(玩家), Card(卡牌), Pile(牌堆), Event(事件)
│   │   └── Models/                 // GameContext(游戏上下文)
│   └── StateMachine/               // 游戏主流程：GameEngine(游戏引擎)
│
├── Rulesets/                       // 规则集插件（在此定义不同的衍生游戏）
│   ├── BaseRuleset.cs             // 规则集基类
│   ├── Shared/                     // 跨版本通用的逻辑
│   │   └── Modes/                  // 模式引擎
│   │       ├── IdentityMode.cs    // 身份模式
│   │       ├── TeamMode.cs        // 团战模式
│   │       └── Mode1v2.cs         // 1v2模式
│   ├── Bang/                       // 经典 Bang! 规则
│   │   └── BangRuleset.cs
│   └── ThreeKingdoms/              // 三国杀规则
│       └── SGSRuleset.cs
│
├── Content/                        // 游戏内容填充（具体的数据实现）
│   ├── Cards/                      // 具体卡牌类：Bang, Beer, Missed, Stagecoach
│   └── Heroes/                     // 英雄技能类：AbilityBase, BartCassidy, SuzieLafayette
│
└── Infrastructure/                 // 基础设施层（支撑引擎运行的外围工具）
    ├── Serialization/              // 存档/读档，GameStateSerializer, JsonCardConverter
    ├── Network/                    // 消息封包、同步协议：MessageBroker, Packet
    └── Config/                     // 卡牌数值配置加载器：CardLoader, ResourceManifest
```

## 模块说明

### Common（通用层）

最底层的模块，不依赖任何其他模块，提供基础的类型定义和接口。

- **Interfaces**: 定义核心接口契约，如实体接口、效果接口、可目标接口
- **Models**: 定义基础数据结构，如坐标、颜色等

### Core（核心层）

游戏引擎的核心逻辑层，纯逻辑实现，不包含UI和具体规则。

- **Domain/Entities**: 定义游戏实体，如玩家、卡牌、牌堆、事件等
- **Domain/Models**: 定义游戏上下文，用于管理游戏状态
- **StateMachine**: 实现游戏引擎，包含事件处理和游戏启动流程

### Rulesets（规则集层）

插件化的规则集系统，支持不同的游戏变体。

- **BaseRuleset**: 规则集基类，定义了规则集的核心接口和方法
- **Shared**: 跨版本通用的逻辑，包含多种游戏模式引擎
  - **IdentityMode**: 经典身份模式，支持警匪、主臣反内等身份系统
  - **TeamMode**: 团战模式，支持队伍对抗玩法
  - **Mode1v2**: 斗地主模式，支持1对2的不对称对战
- **Bang**: 经典《Bang!》规则实现，包含身份模式和胜负逻辑
- **ThreeKingdoms**: 《三国杀》规则实现，包含主臣反内身份系统

### Content（内容层）

具体游戏内容的实现，包括卡牌和英雄。

- **Cards**: 具体的卡牌类实现，如Bang、Beer、Missed、Stagecoach等
- **Heroes**: 英雄技能类实现，包含技能基类和各种英雄的技能

### Infrastructure（基础设施层）

支撑引擎运行的外围工具和服务。

- **Serialization**: 提供游戏状态的序列化和反序列化功能，包括GameStateSerializer和JsonCardConverter
- **Network**: 提供网络通信支持，包括消息代理MessageBroker和数据包Packet
- **Config**: 提供配置加载功能，包括卡牌加载器CardLoader和资源清单ResourceManifest

## 设计原则

1. **分层架构**: 从底层到高层依次为 Common → Core → Rulesets → Content
2. **依赖单向**: 高层模块可以依赖低层模块，低层模块不依赖高层模块
3. **插件化设计**: 规则集采用插件模式，易于扩展新的游戏变体
4. **组件化**: 实体采用组件模式，灵活组合不同功能
5. **纯逻辑核心**: Core 层不包含UI和具体规则，保持引擎的通用性

## 扩展指南

### 添加新游戏模式

1. 在 `Rulesets/Shared/Modes/` 下创建新的模式类
2. 实现模式的胜负条件、角色分配等核心逻辑
3. 在 `Rulesets/` 下的具体规则集中引用该模式

### 添加新规则集

1. 在 `Rulesets/` 下创建新的规则集文件夹
2. 实现规则零件，可复用 Shared 中的模式引擎
3. 在 `Content/` 下添加对应的卡牌和英雄

### 添加新卡牌

1. 在 `Content/Cards/` 下创建新的卡牌类
2. 继承卡牌基类并实现具体效果
3. 在配置文件中定义卡牌数值

### 添加新英雄

1. 在 `Content/Heroes/` 下创建新的英雄类
2. 实现英雄的技能逻辑
3. 在配置文件中定义英雄属性

## 技术栈

- 语言: C#
- 架构: 分层架构 + 组件模式 + 插件模式
- 序列化: JSON
- 配置: CSV/JSON