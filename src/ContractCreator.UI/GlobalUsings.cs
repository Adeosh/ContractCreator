// Системные
global using System;
global using System.IO;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Linq;
global using System.Reactive;
global using System.Reactive.Linq;
global using System.Threading;
global using System.Threading.Tasks;

// Библиотеки
global using ReactiveUI;
global using ReactiveUI.Fody.Helpers;
global using Avalonia.Media.Imaging;
global using Serilog;

// Мои библиотеки
global using ContractCreator.Application;
global using ContractCreator.Infrastructure;
global using ContractCreator.Infrastructure.Services.Gar;
global using ContractCreator.Infrastructure.Services.Bic;
global using ContractCreator.Infrastructure.Services.Classifiers;
global using ContractCreator.Application.Interfaces;
global using ContractCreator.Application.Interfaces.Infrastructure;

// Общие DTO и Exception
global using ContractCreator.Shared.DTOs;
global using ContractCreator.Shared.DTOs.Data;
global using ContractCreator.Shared.Enums;
global using ContractCreator.Shared.Common.Exceptions;
global using ContractCreator.Shared.Common.Extensions;

// Базовые ViewModels и Интерфейсы
global using ContractCreator.UI.Services.Navigation;
global using ContractCreator.UI.Services.Dialogs;
global using ContractCreator.UI.Services.Settings;
global using ContractCreator.UI.Messages;
global using ContractCreator.UI.Helpers;
global using ContractCreator.UI.Views;
global using ContractCreator.UI.ViewModels;
global using ContractCreator.UI.ViewModels.Base;
global using ContractCreator.UI.ViewModels.Contacts;
global using ContractCreator.UI.ViewModels.Firms;
global using ContractCreator.UI.ViewModels.Workers;
global using ContractCreator.UI.ViewModels.Shared;
global using ContractCreator.UI.ViewModels.UserControls;
global using ContractCreator.UI.Views.Shared;
