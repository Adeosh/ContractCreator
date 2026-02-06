using ContractCreator.Shared.DTOs.Data;

namespace ContractCreator.UI.ViewModels.Firms
{
    public class FirmEditorViewModel : ViewModelBase, IParametrizedViewModel
    {
        #region Props
        private readonly IFirmService _firmService;
        private readonly INavigationService _navigation;
        private readonly ISettingsService _settingsService;

        [Reactive] public int Id { get; set; }
        [Reactive] public string FullName { get; set; } = "";
        [Reactive] public string ShortName { get; set; } = "";
        [Reactive] public string Inn { get; set; } = "";
        [Reactive] public string Kpp { get; set; } = "";
        [Reactive] public string Ogrn { get; set; } = "";
        [Reactive] public string Phone { get; set; } = "";
        [Reactive] public string Email { get; set; } = "";
        [Reactive] public string LegalAddressString { get; set; } = "";

        public LegalFormType[] LegalForms => Enum.GetValues<LegalFormType>();
        [Reactive] public LegalFormType SelectedLegalForm { get; set; }

        public TaxationSystemType[] TaxationTypes => Enum.GetValues<TaxationSystemType>();
        [Reactive] public TaxationSystemType SelectedTaxationType { get; set; }
        #endregion
        #region Actions
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }
        #endregion

        public FirmEditorViewModel(
            IFirmService firmService, 
            INavigationService navigation,
            ISettingsService settingsService)
        {
            _firmService = firmService;
            _navigation = navigation;
            _settingsService = settingsService;

            SaveCommand = ReactiveCommand.CreateFromTask(SaveAsync);
            CancelCommand = ReactiveCommand.Create(() => _navigation.NavigateBack());
        }

        public async Task ApplyParameterAsync(object parameter)
        {
            if (parameter is int id)
            {
                if (id != 0) 
                    await LoadAsync(id);
            }
            else if (parameter is EditorParams param)
            {
                if (param.Mode == EditorMode.Edit)
                    await LoadAsync(param.Id);
            }
        }

        public async Task LoadAsync(int firmId)
        {
            try
            {
                var dto = await _firmService.GetFirmByIdAsync(firmId);
                if (dto != null)
                {
                    Id = dto.Id;
                    FullName = dto.FullName;
                    ShortName = dto.ShortName;
                    Inn = dto.INN;
                    Kpp = dto.KPP ?? "";
                    Ogrn = dto.OGRN ?? "";
                    Phone = dto.Phone;
                    Email = dto.Email;
                    SelectedLegalForm = (LegalFormType)dto.LegalFormType;
                    SelectedTaxationType = (TaxationSystemType)dto.TaxationType;
                    LegalAddressString = dto.LegalAddress?.FullAddress ?? "";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при загрузке фирмы!",
                    "Ошибка", UserMessageType.Error);
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                var dto = new FirmDto
                {
                    Id = Id,
                    FullName = FullName,
                    ShortName = ShortName,
                    INN = Inn,
                    KPP = Kpp,
                    OGRN = Ogrn,
                    Phone = Phone,
                    Email = Email,
                    LegalFormType = (byte)SelectedLegalForm,
                    TaxationType = (byte)SelectedTaxationType,
                    LegalAddress = new AddressDto
                    {
                        FullAddress = "обл Ленинградская, г.о. Сосновоборский, г Сосновый Бор, ул Ленинградская",
                        ObjectId = 753471,
                        House = "10",
                        Flat = "1",
                        Building = "A",
                        PostalIndex = "188540",
                    },
                    ActualAddress = new AddressDto
                    {
                        FullAddress = "обл Новосибирская, г.о. город Новосибирск, г Новосибирск, ул Прогулочная",
                        ObjectId = 931166,
                        House = "44",
                        Flat = "100",
                        Building = "3",
                        PostalIndex = "630120",
                    },
                    IsVATPayment = true,
                    CreatedDate = DateOnly.FromDateTime(DateTime.Now),
                    OkopfId = 1,
                    IsDeleted = false
                };

                if (Id == 0)
                {
                    var newId = await _firmService.CreateFirmAsync(dto);

                    if (_settingsService.CurrentFirmId == null)
                        _settingsService.CurrentFirmId = newId;
                }    
                else
                    await _firmService.UpdateFirmAsync(dto);

                _navigation.NavigateBack();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new UserMessageException("Ошибка при сохранении фирмы!",
                    "Ошибка", UserMessageType.Error);
            }
        }
    }
}
