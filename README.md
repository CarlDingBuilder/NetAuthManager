# NetAuthManager  

![GitHub license](https://img.shields.io/github/license/CarlDingBuilder/NetAuthManager?style=flat)
![GitHub stars](https://img.shields.io/github/stars/CarlDingBuilder/NetAuthManager?color=fa6470&style=flat)
![GitHub forks](https://img.shields.io/github/forks/CarlDingBuilder/NetAuthManager?style=flat)

**中文** | [English](./README.en-US.md)

## 简介

**项目未完成，等待提交中……**

`NetAuthManager` 是一个轻量灵活的基于 `.NET` 的权限管理系统，前端采用 Vue.js 驱动。它支持用户管理、角色管理、菜单管理、菜单权限管理以及以基于资源的权限控制功能，同时支持字段级别的权限控制。项目采用 **前后端分离架构**，登录采用 `JWT` 授权模式，便于理解、扩展和集成到其他系统中。

> 本项目前端基于开源项目 [vue-admin-box](https://github.com/cmdparkour/vue-admin-box)
> 为什么前端要基于 `vue-admin-box`，而不是更多 star 的 [vue-pure-admin](https://github.com/pure-admin/vue-pure-admin) 呢？
> 因为 `vue-admin-box` 更简单，我的项目本身是为了体现后端以及前后端交互使用，并不是体现前端功能，如果未来可能的话，我也会考虑做一版 `vue-pure-admin` 的集成项目

**核心功能特点**：  
- **用户管理**：支持新增、编辑、删除和管理系统用户。  
- **角色管理**：创建和配置具有特定权限的用户角色。  
- **菜单管理**：动态控制系统菜单，基于权限显示或隐藏。  
- **权限控制**：支持菜单权限、操作权限及细粒度的数据权限管理。  
- **权限架构**：实现基于角色/用户的权限管理。  
- **前后端分离**：前端基于 Vue3.0，后端基于 .NET8 构建。  

该项目非常适合用于构建后台管理系统、企业级管理工具或复杂的权限管理服务。欢迎大家自由 Fork、贡献代码，或者直接将其作为基础项目进行二次开发！

## 疑问和贡献

如果您发现项目存在问题或者有其他疑问，都可以提一个 Issue。
如果你有好像功能建议和想法，也可以提交一个 Pull Request。

**Pull Request:**

> Fork 代码!\
> 创建自己的分支: `git checkout -b feat/xxxx`\
> 提交您的修改: `git commit -am 'feat(function): add xxxxx'`\
> 推送您的分支: `git push origin feat/xxxx`\
> 提交 pull request

## 声明

本项目同时在 [Github](https://github.com/denisding/NetAuthManager.git) 及 [Gitee](https://gitee.com/bluedman/NetAuthManager.git) 进行管控！

## 许可证

使用 MIT 许可证，完全免费开源
[MIT © 2024-present, Carl.Ding](./LICENSE)