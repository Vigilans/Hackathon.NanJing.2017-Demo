# Hackathon 2017 Bot Demo - Bot Builder Part
## 简介
这是在2017年南京四校 Hackathon 上与 [Chen Junda](https://github.com/viccrubs) 等人合作完成的 Chat Bot 项目中，下层的 Bot Builder 模块 Demo。

另一个模块是 [Bot Framework Wechat Channel](https://github.com/njumsc/BotFrameworkWeChatter) ，负责打通与微信之间的交互通道。

## 使用技术
基本是M$的技术栈……主要技术如下：
* Bot Builder for .Net Core：官方尚未正式支持，目前使用的是 [CXuesong](https://github.com/CXuesong) 的[非官方迁移版本](https://github.com/CXuesong/BotBuilder.Standard)（目前正在接受官方Review）
* LUIS：理解用户输入
* QnA Maker：快速构造简单的FAQ问答
* Azure Cosmos DB：存储指南&与流程的JSON文件
* Azure Search：搜索数据库
* Azure Web App：部署Bot Buider应用

## 其他
与 Bot 相关的 AppID 和 Password 都挂在了这个 Repo 上。

然而在 Azure 上与本次 Hackathon 相关的 Bot 资源已被删除，知道它们也没有什么用了( ﹁ ﹁ ) ~→
