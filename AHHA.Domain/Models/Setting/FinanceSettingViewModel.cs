namespace AHHA.Core.Models.Setting
{
    public class FinanceSettingViewModel
    {
        public Int16 CompanyId { get; set; }
        public Int16 Base_CurrencyId { get; set; }
        public Int16 Local_CurrencyId { get; set; }
        public Int32 ExhGainLoss_GlId { get; set; }
        public Int32 BankCharge_GlId { get; set; }
        public Int32 ProfitLoss_GlId { get; set; }
        public Int32 RetEarning_GlId { get; set; }
        public Int32 SaleGst_GlId { get; set; }
        public Int32 PurGst_GlId { get; set; }
        public Int32 SaleDef_GlId { get; set; }
        public Int32 PurDef_GlId { get; set; }
        public Int16 CreateById { get; set; }
        public DateTime CreateDate { get; set; }
        public Int16? EditById { get; set; }
        public DateTime? EditDate { get; set; }
        public string CreateBy { get; set; }
        public string EditBy { get; set; }
    }
}