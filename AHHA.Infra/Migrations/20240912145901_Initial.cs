using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AHHA.Infra.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdmAuditLog",
                columns: table => new
                {
                    AuditId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ModuleId = table.Column<short>(type: "smallint", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TblName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeId = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateById = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmAuditLog", x => x.AuditId);
                });

            migrationBuilder.CreateTable(
                name: "AdmCompany",
                columns: table => new
                {
                    CompanyId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegistrationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxRegistrationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddressId = table.Column<long>(type: "bigint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmCompany", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "AdmErrorLog",
                columns: table => new
                {
                    ErrId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ModuleId = table.Column<short>(type: "smallint", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TblName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModeId = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateById = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmErrorLog", x => x.ErrId);
                });

            migrationBuilder.CreateTable(
                name: "AdmModule",
                columns: table => new
                {
                    ModuleId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModuleCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmModule", x => x.ModuleId);
                });

            migrationBuilder.CreateTable(
                name: "AdmShareData",
                columns: table => new
                {
                    ModuleId = table.Column<short>(type: "smallint", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    SetId = table.Column<short>(type: "smallint", nullable: false),
                    ShareToAll = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmShareData", x => new { x.ModuleId, x.TransactionId, x.CompanyId, x.SetId });
                });

            migrationBuilder.CreateTable(
                name: "AdmTransaction",
                columns: table => new
                {
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    ModuleId = table.Column<short>(type: "smallint", nullable: false),
                    TransactionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransCategoryId = table.Column<int>(type: "int", nullable: false),
                    IsNumber = table.Column<bool>(type: "bit", nullable: false),
                    SeqNo = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<int>(type: "int", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmTransaction", x => new { x.ModuleId, x.TransactionId });
                });

            migrationBuilder.CreateTable(
                name: "AdmTransactionCategory",
                columns: table => new
                {
                    TransCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmTransactionCategory", x => x.TransCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "AdmUser",
                columns: table => new
                {
                    UserId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserPassword = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserGroupId = table.Column<short>(type: "smallint", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmUser", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AdmUserGroup",
                columns: table => new
                {
                    UserGroupId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserGroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmUserGroup", x => x.UserGroupId);
                });

            migrationBuilder.CreateTable(
                name: "AdmUserGroupRights",
                columns: table => new
                {
                    UserGroupId = table.Column<short>(type: "smallint", nullable: false),
                    ModuleId = table.Column<short>(type: "smallint", nullable: false),
                    TransactionId = table.Column<short>(type: "smallint", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsCreate = table.Column<bool>(type: "bit", nullable: false),
                    IsEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    IsExport = table.Column<bool>(type: "bit", nullable: false),
                    IsPrint = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmUserGroupRights", x => new { x.UserGroupId, x.ModuleId, x.TransactionId });
                });

            migrationBuilder.CreateTable(
                name: "AdmUserLog",
                columns: table => new
                {
                    UserId = table.Column<short>(type: "smallint", nullable: false),
                    LoginDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLogin = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmUserLog", x => new { x.UserId, x.LoginDate });
                });

            migrationBuilder.CreateTable(
                name: "AdmUserRights",
                columns: table => new
                {
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    UserId = table.Column<short>(type: "smallint", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    UserGroupId = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmUserRights", x => new { x.CompanyId, x.UserId });
                });

            migrationBuilder.CreateTable(
                name: "ArInvoiceDt",
                columns: table => new
                {
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    ItemNo = table.Column<short>(type: "smallint", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<short>(type: "smallint", nullable: false),
                    DocItemNo = table.Column<short>(type: "smallint", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    GLId = table.Column<short>(type: "smallint", nullable: false),
                    QTY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BillQTY = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UomId = table.Column<byte>(type: "tinyint", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotLocalAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotCurAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GstId = table.Column<byte>(type: "tinyint", nullable: false),
                    GstPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstLocalAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstCurAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DepartmentId = table.Column<short>(type: "smallint", nullable: false),
                    EmployeeId = table.Column<short>(type: "smallint", nullable: false),
                    PortId = table.Column<short>(type: "smallint", nullable: false),
                    VesselId = table.Column<int>(type: "int", nullable: false),
                    BargeId = table.Column<short>(type: "smallint", nullable: false),
                    VoyageId = table.Column<int>(type: "int", nullable: false),
                    OperationId = table.Column<long>(type: "bigint", nullable: false),
                    OperationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OPRefNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalesOrderId = table.Column<long>(type: "bigint", nullable: false),
                    SalesOrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplyDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuppInvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    APInvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    APInvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditVersion = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArInvoiceDt", x => new { x.InvoiceId, x.ItemNo });
                });

            migrationBuilder.CreateTable(
                name: "ArInvoiceHd",
                columns: table => new
                {
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrnDate = table.Column<DateOnly>(type: "date", nullable: false),
                    AccountDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DeliveryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    ExhRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CtyExhRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditTermId = table.Column<short>(type: "smallint", nullable: false),
                    BankId = table.Column<int>(type: "int", nullable: false),
                    TotAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotLocalAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotCtyAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstClaimDate = table.Column<DateOnly>(type: "date", nullable: false),
                    GstAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstLocalAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GstCtyAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotAmtAftGst = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotLocalAmtAftGst = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotCtyAmtAftGst = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SalesOrderId = table.Column<long>(type: "bigint", nullable: false),
                    SalesOrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationId = table.Column<long>(type: "bigint", nullable: false),
                    OperationNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModuleFrom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuppInvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    APInvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    APInvoiceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsCancel = table.Column<bool>(type: "bit", nullable: false),
                    CancelById = table.Column<int>(type: "int", nullable: true),
                    CancelDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelRemarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditVersion = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArInvoiceHd", x => x.InvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "M_AccountGroup",
                columns: table => new
                {
                    AccGroupId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    AccGroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_AccountGroup", x => x.AccGroupId);
                });

            migrationBuilder.CreateTable(
                name: "M_AccountSetup",
                columns: table => new
                {
                    AccSetupId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    AccSetupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccSetupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccSetupCategoryId = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_AccountSetup", x => x.AccSetupId);
                });

            migrationBuilder.CreateTable(
                name: "M_AccountSetupCategory",
                columns: table => new
                {
                    AccSetupCategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccSetupCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccSetupCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_AccountSetupCategory", x => x.AccSetupCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_AccountType",
                columns: table => new
                {
                    AccTypeId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    AccTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<short>(type: "smallint", nullable: false),
                    AccGroupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_AccountType", x => x.AccTypeId);
                });

            migrationBuilder.CreateTable(
                name: "M_Bank",
                columns: table => new
                {
                    BankId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SwiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GLId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Bank", x => x.BankId);
                });

            migrationBuilder.CreateTable(
                name: "M_Barge",
                columns: table => new
                {
                    BargeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    BargeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BargeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallSign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMOCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GRT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BargeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Barge", x => x.BargeId);
                });

            migrationBuilder.CreateTable(
                name: "M_Category",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_ChartOfAccount",
                columns: table => new
                {
                    GLId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    GLCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GLName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccTypeId = table.Column<short>(type: "smallint", nullable: false),
                    AccGroupId = table.Column<short>(type: "smallint", nullable: false),
                    COACategoryId1 = table.Column<short>(type: "smallint", nullable: false),
                    COACategoryId2 = table.Column<short>(type: "smallint", nullable: false),
                    COACategoryId3 = table.Column<short>(type: "smallint", nullable: false),
                    IsSysControl = table.Column<bool>(type: "bit", nullable: false),
                    SeqNo = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_ChartOfAccount", x => x.GLId);
                });

            migrationBuilder.CreateTable(
                name: "M_COACategory1",
                columns: table => new
                {
                    COACategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    COACategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COACategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_COACategory1", x => x.COACategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_COACategory2",
                columns: table => new
                {
                    COACategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    COACategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COACategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_COACategory2", x => x.COACategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_COACategory3",
                columns: table => new
                {
                    COACategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    COACategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COACategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_COACategory3", x => x.COACategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_Country",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Country", x => x.CountryId);
                });

            migrationBuilder.CreateTable(
                name: "M_CreditTerm",
                columns: table => new
                {
                    CreditTermId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    CreditTermCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditTermName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoDays = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CreditTerm", x => x.CreditTermId);
                });

            migrationBuilder.CreateTable(
                name: "M_CreditTermDt",
                columns: table => new
                {
                    CreditTermId = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    FromDay = table.Column<short>(type: "smallint", nullable: false),
                    ToDay = table.Column<short>(type: "smallint", nullable: false),
                    IsEndOfMonth = table.Column<bool>(type: "bit", nullable: false),
                    DueDay = table.Column<short>(type: "smallint", nullable: false),
                    NoMonth = table.Column<short>(type: "smallint", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CreditTermDt", x => new { x.CreditTermId, x.CompanyId, x.FromDay });
                });

            migrationBuilder.CreateTable(
                name: "M_Currency",
                columns: table => new
                {
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMultiply = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Currency", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "M_CurrencyDt",
                columns: table => new
                {
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ExhRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CurrencyDt", x => new { x.CurrencyId, x.CompanyId, x.ValidFrom });
                });

            migrationBuilder.CreateTable(
                name: "M_CurrencyLocalDt",
                columns: table => new
                {
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    ExhRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CurrencyLocalDt", x => new { x.CurrencyId, x.CompanyId, x.ValidFrom });
                });

            migrationBuilder.CreateTable(
                name: "M_Customer",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerOtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerRegNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CreditTermId = table.Column<short>(type: "smallint", nullable: false),
                    ParentCustomerId = table.Column<int>(type: "int", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    IsVendor = table.Column<bool>(type: "bit", nullable: false),
                    IsTrader = table.Column<bool>(type: "bit", nullable: false),
                    IsSupplier = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Customer", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "M_CustomerAddress",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefaultAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleveryAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsFinAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsSalesAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CustomerAddress", x => new { x.CustomerId, x.AddressId });
                });

            migrationBuilder.CreateTable(
                name: "M_CustomerContact",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OffNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactMessType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsFinance = table.Column<bool>(type: "bit", nullable: false),
                    IsSales = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CustomerContact", x => new { x.CustomerId, x.ContactId });
                });

            migrationBuilder.CreateTable(
                name: "M_CustomerCreditLimit",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    EffectFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectUntil = table.Column<DateOnly>(type: "date", nullable: false),
                    IsExpires = table.Column<bool>(type: "bit", nullable: false),
                    CreditLimitAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CustomerCreditLimit", x => new { x.CustomerId, x.CompanyId, x.EffectFrom });
                });

            migrationBuilder.CreateTable(
                name: "M_CustomerGroupCreditLimit",
                columns: table => new
                {
                    GroupCreditLimitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    GroupCreditLimitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupCreditLimitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CustomerGroupCreditLimit", x => x.GroupCreditLimitId);
                });

            migrationBuilder.CreateTable(
                name: "M_Department",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Department", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "M_Designation",
                columns: table => new
                {
                    DesignationId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    DesignationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Designation", x => x.DesignationId);
                });

            migrationBuilder.CreateTable(
                name: "M_Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeOtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeePhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeSex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MartialStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeDOB = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeJoinDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeLastDate = table.Column<DateOnly>(type: "date", nullable: false),
                    EmployeeOffEmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeOtherEmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Employee", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "M_GroupCreditLimit",
                columns: table => new
                {
                    GroupCreditLimitId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    GroupCreditLimitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupCreditLimitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_GroupCreditLimit", x => x.GroupCreditLimitId);
                });

            migrationBuilder.CreateTable(
                name: "M_GroupCreditLimit_Customer",
                columns: table => new
                {
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    GroupCreditLimitId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_GroupCreditLimit_Customer", x => new { x.CompanyId, x.GroupCreditLimitId, x.CustomerId });
                });

            migrationBuilder.CreateTable(
                name: "M_GroupCreditLimitDt",
                columns: table => new
                {
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    GroupCreditLimitId = table.Column<int>(type: "int", nullable: false),
                    EffectFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    EffectUntil = table.Column<DateOnly>(type: "date", nullable: false),
                    IsExpires = table.Column<bool>(type: "bit", nullable: false),
                    CreditLimitAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_GroupCreditLimitDt", x => new { x.GroupCreditLimitId, x.CompanyId, x.EffectFrom });
                });

            migrationBuilder.CreateTable(
                name: "M_Gst",
                columns: table => new
                {
                    GstId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    GstCategoryId = table.Column<short>(type: "smallint", nullable: false),
                    GstCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Gst", x => x.GstId);
                });

            migrationBuilder.CreateTable(
                name: "M_GstCategory",
                columns: table => new
                {
                    GstCategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    GstCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GstCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_GstCategory", x => x.GstCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_GstDt",
                columns: table => new
                {
                    GstId = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    GstPercentahge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_GstDt", x => new { x.GstId, x.CompanyId, x.ValidFrom });
                });

            migrationBuilder.CreateTable(
                name: "M_OrderType",
                columns: table => new
                {
                    OrderTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    OrderTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderTypeCategoryId = table.Column<short>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_OrderType", x => x.OrderTypeId);
                });

            migrationBuilder.CreateTable(
                name: "M_OrderTypeCategory",
                columns: table => new
                {
                    OrderTypeCategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    OrderTypeCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderTypeCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_OrderTypeCategory", x => x.OrderTypeCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_PaymentType",
                columns: table => new
                {
                    PaymentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    PaymentTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_PaymentType", x => x.PaymentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "M_Port",
                columns: table => new
                {
                    PortId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    PortRegionId = table.Column<int>(type: "int", nullable: false),
                    PortCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Port", x => x.PortId);
                });

            migrationBuilder.CreateTable(
                name: "M_PortRegion",
                columns: table => new
                {
                    PortRegionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    PortRegionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortRegionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_PortRegion", x => x.PortRegionId);
                });

            migrationBuilder.CreateTable(
                name: "M_Product",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Product", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "M_SubCategory",
                columns: table => new
                {
                    SubCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_SubCategory", x => x.SubCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_Supplier",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    SupplierCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierOtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierRegNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CreditTermId = table.Column<short>(type: "smallint", nullable: false),
                    ParentSupplierId = table.Column<int>(type: "int", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    IsVendor = table.Column<bool>(type: "bit", nullable: false),
                    IsTrader = table.Column<bool>(type: "bit", nullable: false),
                    IsSupplier = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Supplier", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "M_SupplierAddress",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefaultAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsDeliveryAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsFinAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsSalesAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_SupplierAddress", x => new { x.SupplierId, x.AddressId });
                });

            migrationBuilder.CreateTable(
                name: "M_SupplierContact",
                columns: table => new
                {
                    ContactId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OffNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MessId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactMessType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    IsFinance = table.Column<bool>(type: "bit", nullable: false),
                    IsSales = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_SupplierContact", x => new { x.SupplierId, x.ContactId });
                });

            migrationBuilder.CreateTable(
                name: "M_Tax",
                columns: table => new
                {
                    TaxId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    TaxCategoryId = table.Column<short>(type: "smallint", nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Tax", x => x.TaxId);
                });

            migrationBuilder.CreateTable(
                name: "M_TaxCategory",
                columns: table => new
                {
                    TaxCategoryId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    TaxCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_TaxCategory", x => x.TaxCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_TaxDt",
                columns: table => new
                {
                    TaxId = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ValidFrom = table.Column<DateOnly>(type: "date", nullable: false),
                    TaxPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_TaxDt", x => new { x.TaxId, x.CompanyId, x.ValidFrom });
                });

            migrationBuilder.CreateTable(
                name: "M_Uom",
                columns: table => new
                {
                    UomId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    UomCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Uom", x => x.UomId);
                });

            migrationBuilder.CreateTable(
                name: "M_UomDt",
                columns: table => new
                {
                    UomId = table.Column<short>(type: "smallint", nullable: false),
                    PackUomId = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    UomFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_UomDt", x => new { x.UomId, x.PackUomId });
                });

            migrationBuilder.CreateTable(
                name: "M_Vessel",
                columns: table => new
                {
                    VesselId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    VesselCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallSign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMOCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GRT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Vessel", x => x.VesselId);
                });

            migrationBuilder.CreateTable(
                name: "M_Voyage",
                columns: table => new
                {
                    VoyageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    VoyageNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselId = table.Column<int>(type: "int", nullable: false),
                    BargeId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Voyage", x => x.VoyageId);
                });

            migrationBuilder.CreateTable(
                name: "S_DecSettings",
                columns: table => new
                {
                    CompanyId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AmtDec = table.Column<short>(type: "smallint", nullable: false),
                    LocAmtDec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceDec = table.Column<short>(type: "smallint", nullable: false),
                    QtyDec = table.Column<short>(type: "smallint", nullable: false),
                    ExhRateDec = table.Column<short>(type: "smallint", nullable: false),
                    DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_DecSettings", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "S_FinSettings",
                columns: table => new
                {
                    CompanyId = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Base_CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    Local_CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    ExhGainLoss_GlId = table.Column<int>(type: "int", nullable: false),
                    BankCharge_GlId = table.Column<int>(type: "int", nullable: false),
                    ProfitLoss_GlId = table.Column<int>(type: "int", nullable: false),
                    RetEarning_GlId = table.Column<int>(type: "int", nullable: false),
                    SaleGst_GlId = table.Column<int>(type: "int", nullable: false),
                    PurGst_GlId = table.Column<int>(type: "int", nullable: false),
                    SaleDef_GlId = table.Column<int>(type: "int", nullable: false),
                    PurDef_GlId = table.Column<int>(type: "int", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_FinSettings", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "S_NumberFormat",
                columns: table => new
                {
                    NumberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<short>(type: "smallint", nullable: false),
                    ModuleId = table.Column<short>(type: "smallint", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrefixSeq = table.Column<short>(type: "smallint", nullable: false),
                    PrefixDelimiter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncludeYear = table.Column<bool>(type: "bit", nullable: false),
                    YearSeq = table.Column<short>(type: "smallint", nullable: false),
                    YearFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YearDelimiter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncludeMonth = table.Column<bool>(type: "bit", nullable: false),
                    MonthSeq = table.Column<short>(type: "smallint", nullable: false),
                    MonthFormat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MonthDelimiter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoDIgits = table.Column<short>(type: "smallint", nullable: false),
                    DIgitSeq = table.Column<short>(type: "smallint", nullable: false),
                    ResetYearly = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<short>(type: "smallint", nullable: false),
                    EditById = table.Column<short>(type: "smallint", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_S_NumberFormat", x => x.NumberId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmAuditLog");

            migrationBuilder.DropTable(
                name: "AdmCompany");

            migrationBuilder.DropTable(
                name: "AdmErrorLog");

            migrationBuilder.DropTable(
                name: "AdmModule");

            migrationBuilder.DropTable(
                name: "AdmShareData");

            migrationBuilder.DropTable(
                name: "AdmTransaction");

            migrationBuilder.DropTable(
                name: "AdmTransactionCategory");

            migrationBuilder.DropTable(
                name: "AdmUser");

            migrationBuilder.DropTable(
                name: "AdmUserGroup");

            migrationBuilder.DropTable(
                name: "AdmUserGroupRights");

            migrationBuilder.DropTable(
                name: "AdmUserLog");

            migrationBuilder.DropTable(
                name: "AdmUserRights");

            migrationBuilder.DropTable(
                name: "ArInvoiceDt");

            migrationBuilder.DropTable(
                name: "ArInvoiceHd");

            migrationBuilder.DropTable(
                name: "M_AccountGroup");

            migrationBuilder.DropTable(
                name: "M_AccountSetup");

            migrationBuilder.DropTable(
                name: "M_AccountSetupCategory");

            migrationBuilder.DropTable(
                name: "M_AccountType");

            migrationBuilder.DropTable(
                name: "M_Bank");

            migrationBuilder.DropTable(
                name: "M_Barge");

            migrationBuilder.DropTable(
                name: "M_Category");

            migrationBuilder.DropTable(
                name: "M_ChartOfAccount");

            migrationBuilder.DropTable(
                name: "M_COACategory1");

            migrationBuilder.DropTable(
                name: "M_COACategory2");

            migrationBuilder.DropTable(
                name: "M_COACategory3");

            migrationBuilder.DropTable(
                name: "M_Country");

            migrationBuilder.DropTable(
                name: "M_CreditTerm");

            migrationBuilder.DropTable(
                name: "M_CreditTermDt");

            migrationBuilder.DropTable(
                name: "M_Currency");

            migrationBuilder.DropTable(
                name: "M_CurrencyDt");

            migrationBuilder.DropTable(
                name: "M_CurrencyLocalDt");

            migrationBuilder.DropTable(
                name: "M_Customer");

            migrationBuilder.DropTable(
                name: "M_CustomerAddress");

            migrationBuilder.DropTable(
                name: "M_CustomerContact");

            migrationBuilder.DropTable(
                name: "M_CustomerCreditLimit");

            migrationBuilder.DropTable(
                name: "M_CustomerGroupCreditLimit");

            migrationBuilder.DropTable(
                name: "M_Department");

            migrationBuilder.DropTable(
                name: "M_Designation");

            migrationBuilder.DropTable(
                name: "M_Employee");

            migrationBuilder.DropTable(
                name: "M_GroupCreditLimit");

            migrationBuilder.DropTable(
                name: "M_GroupCreditLimit_Customer");

            migrationBuilder.DropTable(
                name: "M_GroupCreditLimitDt");

            migrationBuilder.DropTable(
                name: "M_Gst");

            migrationBuilder.DropTable(
                name: "M_GstCategory");

            migrationBuilder.DropTable(
                name: "M_GstDt");

            migrationBuilder.DropTable(
                name: "M_OrderType");

            migrationBuilder.DropTable(
                name: "M_OrderTypeCategory");

            migrationBuilder.DropTable(
                name: "M_PaymentType");

            migrationBuilder.DropTable(
                name: "M_Port");

            migrationBuilder.DropTable(
                name: "M_PortRegion");

            migrationBuilder.DropTable(
                name: "M_Product");

            migrationBuilder.DropTable(
                name: "M_SubCategory");

            migrationBuilder.DropTable(
                name: "M_Supplier");

            migrationBuilder.DropTable(
                name: "M_SupplierAddress");

            migrationBuilder.DropTable(
                name: "M_SupplierContact");

            migrationBuilder.DropTable(
                name: "M_Tax");

            migrationBuilder.DropTable(
                name: "M_TaxCategory");

            migrationBuilder.DropTable(
                name: "M_TaxDt");

            migrationBuilder.DropTable(
                name: "M_Uom");

            migrationBuilder.DropTable(
                name: "M_UomDt");

            migrationBuilder.DropTable(
                name: "M_Vessel");

            migrationBuilder.DropTable(
                name: "M_Voyage");

            migrationBuilder.DropTable(
                name: "S_DecSettings");

            migrationBuilder.DropTable(
                name: "S_FinSettings");

            migrationBuilder.DropTable(
                name: "S_NumberFormat");
        }
    }
}