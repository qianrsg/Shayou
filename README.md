# Bang! 游戏引擎项目结构

## 项目概述

这是一个模块化的卡牌游戏引擎，支持《Bang!》及其衍生游戏（如《三国杀》等）。项目采用分层架构设计，实现了核心逻辑与具体规则的解耦。

## 目录结构

```
├── Config/                         // 配置文件
│   └── Piles/                      // 牌堆配置
│       └── standard.json           // 标准牌堆配置
│
├── Core/                           // 核心引擎大脑（纯逻辑，无UI，无具体规则）
│   ├── Domain/                     // 领域层
│   │   ├── Entities/               // 实体：Player(玩家), Card(卡牌), Zone(区域)
│   │   ├── Events/                 // 事件：Event, BaseEvent, AtomicEvent
│   │   └── Models/                 // GameContext(游戏上下文), IContext(上下文接口)
│   └── GameEngine.cs               // 游戏引擎：事件处理和游戏启动流程
│
├── Rulesets/                       // 规则集插件（在此定义不同的衍生游戏）
│   ├── BaseRuleset.cs              // 规则集基类
│   ├── Shared/                     // 跨版本通用的逻辑
│   │   └── Modes/                  // 模式引擎
│   │       ├── IdentityMode.cs     // 身份模式
│   │       ├── TeamMode.cs         // 团战模式
│   │       └── Mode1v2.cs          // 1v2模式
│   ├── Bang/                       // 经典 Bang! 规则
│   │   └── BangRuleset.cs
│   └── ThreeKingdoms/              // 三国杀规则
│       └── SGSRuleset.cs
│
└── Content/                        // 游戏内容填充（具体的数据实现）
    ├── Cards/                      // 具体卡牌类：Bang, Beer, Missed, Stagecoach
    └── Heroes/                     // 英雄技能类：AbilityBase, BartCassidy, SuzieLafayette
```

## 模块说明

### Config（配置层）

游戏配置文件，存储牌堆、卡牌数值等静态数据。

- **Piles**: 牌堆配置文件，定义标准牌堆的卡牌组成

### Core（核心层）

游戏引擎的核心逻辑层，纯逻辑实现，不包含UI和具体规则。

- **Domain/Entities**: 定义游戏实体，包括 Player（玩家）、Card（卡牌）、Zone（区域）
- **Domain/Events**: 定义事件系统，包括 Event、BaseEvent、AtomicEvent，用于游戏流程控制
- **Domain/Models**: 定义游戏上下文 GameContext 和接口 IContext，管理游戏状态
- **GameEngine**: 游戏引擎核心，包含事件栈处理和游戏启动流程

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

## 设计原则

1. **分层架构**: 从底层到高层依次为 Config → Core → Rulesets → Content
2. **依赖单向**: 高层模块可以依赖低层模块，低层模块不依赖高层模块
3. **插件化设计**: 规则集采用插件模式，易于扩展新的游戏变体
4. **事件驱动**: 通过事件栈驱动游戏流程，支持复杂的卡牌交互
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