# NetAuthManager

![GitHub license](https://img.shields.io/github/license/CarlDingBuilder/NetAuthManager?style=flat)
![GitHub stars](https://img.shields.io/github/stars/CarlDingBuilder/NetAuthManager?color=fa6470&style=flat)
![GitHub forks](https://img.shields.io/github/forks/CarlDingBuilder/NetAuthManager?style=flat)

[中文](README.md) | **English**

## Introduction

**The project is not completed and is pending submission...**

`NetAuthManager` is a lightweight and flexible .NET-based authorization management system, with a frontend powered by Vue.js. It supports user management, role management, menu management, menu permission management, resource-based authorization, and field-level permission control. The project is designed with a frontend-backend separation architecture, utilizing `JWT` for login authentication, making it easy to understand, extend, and integrate into other systems.

> The frontend of this project is based on the open-source project [vue-admin-box](https://github.com/cmdparkour/vue-admin-box).  
> Why is the frontend based on `vue-admin-box`, instead of the more popular [vue-pure-admin](https://github.com/pure-admin/vue-pure-admin)?  
> The reason is that `vue-admin-box` is simpler. The primary goal of my project is to demonstrate backend functionality and the interaction between frontend and backend, rather than showcasing the capabilities of the frontend. However, if possible in the future, I might also consider creating an integrated version with `vue-pure-admin`.

**Key Features:**
- **User Management**: Supports adding, editing, deleting, and managing system users.
- **Role Management**: Create and configure user roles with specific permissions.
- **Menu Management**: Dynamically control system menus, showing or hiding them based on permissions.
- **Authorization Control**: Supports menu permissions, operational permissions, and fine-grained data permission management.
- **Permission Architecture**: Implements role-based、user-based access control.
- **Frontend-Backend Separation**: Frontend built with Vue 3.0; backend built with .NET 8.
This project is ideal for building admin management systems, enterprise-level management tools, or complex authorization management services. Feel free to fork, contribute, or use it as a base for your own projects!

## Questions and Contributions  

If you encounter any issues with the project or have other questions, feel free to open an Issue.  
If you have feature suggestions or ideas, you are also welcome to submit a Pull Request.  

**Pull Request:**
> Fork the repository!\
> Create your own branch: `git checkout -b feat/xxxx`\
> Commit your changes: `git commit -am 'feat(function): add xxxxx'`\
> Push your branch: `git push origin feat/xxxx`\
> Submit a Pull Request

## Statement  

This project is managed simultaneously on both [Github](https://github.com/denisding/NetAuthManager.git) and [Gitee](https://gitee.com/bluedman/NetAuthManager.git)!

## License

Licensed under the MIT License, completely free and open-source.
[MIT © 2024-present, Carl.Ding](./LICENSE)