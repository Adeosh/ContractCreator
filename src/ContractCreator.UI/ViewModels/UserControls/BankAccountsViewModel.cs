namespace ContractCreator.UI.ViewModels.UserControls
{
    public class BankAccountsViewModel : ViewModelBase
    {
        #region Props
        private readonly IBankAccountService _accountService;
        private readonly IBicService _classifierService;
        private readonly IUserDialogService _dialogService;
        private int _ownerId;
        private OwnerType _ownerType;

        [Reactive] public bool IsListMode { get; set; } = true;
        [Reactive] public bool IsBusy { get; set; }

        public ObservableCollection<BankAccountDto> Accounts { get; } = new();
        [Reactive] public BankAccountDto? SelectedAccount { get; set; }

        [Reactive] public int EditingId { get; set; }
        [Reactive] public string SearchBicText { get; set; } = ""; // Текст в поиске
        public ObservableCollection<BankDto> FoundBanks { get; } = new();
        [Reactive] public bool IsBankDropDownOpen { get; set; }

        [Reactive] public string Bic { get; set; } = "";
        [Reactive] public string BankName { get; set; } = "";
        [Reactive] public string BankAddress { get; set; } = "";
        [Reactive] public string CorrespondentAccount { get; set; } = "";
        [Reactive] public string AccountNumber { get; set; } = "";
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> AddCommand { get; }
        public ReactiveCommand<Unit, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> DeleteCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        #endregion

        public BankAccountsViewModel(
            IBankAccountService accountService,
            IBicService classifierService,
            IUserDialogService dialogService)
        {
            _accountService = accountService;
            _classifierService = classifierService;
            _dialogService = dialogService;

            SetupBank();

            AddCommand = ReactiveCommand.Create(StartAdd);

            EditCommand = ReactiveCommand.Create(StartEdit,
                this.WhenAnyValue(x => x.SelectedAccount).Select(x => x != null));

            DeleteCommand = ReactiveCommand.CreateFromTask(DeleteAsync,
                this.WhenAnyValue(x => x.SelectedAccount).Select(x => x != null));

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync,
                this.WhenAnyValue(x => x.AccountNumber, x => x.Bic)
                    .Select(t => !string.IsNullOrWhiteSpace(t.Item1) && !string.IsNullOrWhiteSpace(t.Item2)));

            CancelCommand = ReactiveCommand.Create(() => { IsListMode = true; });
        }

        private void SetupBank()
        {
            this.WhenAnyValue(x => x.SearchBicText)
                .Skip(1)
                .Throttle(TimeSpan.FromMilliseconds(400), RxApp.TaskpoolScheduler)
                .Select(t => t?.Trim())
                .DistinctUntilChanged()
                .Where(t => !string.IsNullOrEmpty(t) && t.Length >= 3 && !IsListMode)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async t => await PerformSearch(t!));
        }

        public async Task LoadDataAsync(int ownerId, OwnerType ownerType)
        {
            _ownerId = ownerId;
            _ownerType = ownerType;
            await RefreshListAsync();
        }

        private async Task RefreshListAsync()
        {
            if (_ownerId == 0)
            {
                Accounts.Clear();
                return;
            }

            IsBusy = true;

            try
            {
                IEnumerable<BankAccountDto> list;

                if (_ownerType == OwnerType.Firm)
                    list = await _accountService.GetByFirmIdAsync(_ownerId);
                else
                    list = await _accountService.GetByCounterpartyIdAsync(_ownerId);

                foreach (var item in list)
                    Accounts.Add(item);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка загрузки счетов");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PerformSearch(string query)
        {
            try
            {
                var results = await _classifierService.SearchAsync(query);
                FoundBanks.Clear();
                foreach (var b in results) 
                    FoundBanks.Add(b);

                IsBankDropDownOpen = FoundBanks.Any();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка поиска банка");
            }
        }

        public void SelectBankFromClassifier(BankDto bank)
        {
            if (bank == null) return;

            Bic = bank.Bic;
            BankName = bank.Name;
            BankAddress = bank.Address;
            CorrespondentAccount = bank.CorrespondentAccount;

            SearchBicText = bank.Bic;
            IsBankDropDownOpen = false;
        }

        private void StartAdd()
        {
            EditingId = 0;
            SearchBicText = "";
            Bic = "";
            BankName = "";
            BankAddress = "";
            CorrespondentAccount = "";
            AccountNumber = "";
            IsListMode = false;
        }

        private void StartEdit()
        {
            if (SelectedAccount == null) return;
            var dto = SelectedAccount;

            EditingId = dto.Id;
            SearchBicText = dto.BIC;
            Bic = dto.BIC;
            BankName = dto.BankName;
            BankAddress = dto.BankAddress ?? "";
            CorrespondentAccount = dto.CorrespondentAccount ?? "";
            AccountNumber = dto.AccountNumber;

            IsListMode = false;
        }

        public async Task CommitPendingAccountsAsync(int newOwnerId)
        {
            _ownerId = newOwnerId;

            var pendingAccounts = Accounts.Where(x => x.Id == 0).ToList();
            if (!pendingAccounts.Any()) return;

            IsBusy = true;

            try
            {
                foreach (var acc in pendingAccounts)
                {
                    if (_ownerType == OwnerType.Firm)
                        acc.FirmId = newOwnerId;
                    else 
                        acc.CounterpartyId = newOwnerId;

                    await _accountService.CreateAsync(acc);
                }

                await RefreshListAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка сохранения черновиков счетов");
                await _dialogService.ShowErrorAsync("Не удалось сохранить банковские счета новой фирмы.");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                IsBusy = true;

                var dto = new BankAccountDto
                {
                    Id = EditingId,
                    BIC = Bic,
                    BankName = BankName,
                    BankAddress = BankAddress,
                    CorrespondentAccount = CorrespondentAccount,
                    AccountNumber = AccountNumber,
                    IsDeleted = false
                };

                if (_ownerType == OwnerType.Firm)
                {
                    dto.FirmId = _ownerId == 0 ? null : _ownerId;
                    dto.CounterpartyId = null;
                }
                else
                {
                    dto.FirmId = null;
                    dto.CounterpartyId = _ownerId == 0 ? null : _ownerId;
                }

                if (_ownerId != 0)
                {
                    if (EditingId == 0)
                        await _accountService.CreateAsync(dto);
                    else
                        await _accountService.UpdateAsync(dto);

                    await RefreshListAsync();
                }
                else
                {
                    if (EditingId == 0)
                    {
                        Accounts.Add(dto);
                    }
                    else
                    {
                        var itemToUpdate = Accounts.FirstOrDefault(x => x == SelectedAccount);
                        if (itemToUpdate != null)
                        {
                            itemToUpdate.BIC = dto.BIC;
                            itemToUpdate.BankName = dto.BankName;
                            itemToUpdate.AccountNumber = dto.AccountNumber;
                            itemToUpdate.CorrespondentAccount = dto.CorrespondentAccount;
                            itemToUpdate.BankAddress = dto.BankAddress;

                            int index = Accounts.IndexOf(itemToUpdate);
                            Accounts[index] = dto;
                        }
                    }
                }

                IsListMode = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка сохранения счета");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task DeleteAsync()
        {
            if (SelectedAccount == null) return;

            bool isConfirmed = await _dialogService.ShowConfirmationAsync(
                $"Вы действительно хотите удалить счет в банке \"{SelectedAccount.BankName}\"?\nЭто действие нельзя отменить.",
                "Удаление счета");

            if (!isConfirmed)
                return;

            try
            {
                IsBusy = true;

                if (SelectedAccount.Id != 0)
                    await _accountService.DeleteAsync(SelectedAccount.Id);

                Accounts.Remove(SelectedAccount);
                SelectedAccount = null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка удаления");
            }
        }
    }
}
