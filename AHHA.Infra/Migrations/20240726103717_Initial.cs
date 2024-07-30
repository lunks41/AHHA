﻿using System;
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
                name: "M_AccountSetup",
                columns: table => new
                {
                    AccSetupId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    AccSetupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccSetupName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccSetupCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_AccountSetup", x => x.AccSetupId);
                });

            migrationBuilder.CreateTable(
                name: "M_AccountSetupCategory",
                columns: table => new
                {
                    AccSetupCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    AccSetupCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccSetupCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_AccountSetupCategory", x => x.AccSetupCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_Bank",
                columns: table => new
                {
                    BankId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    AccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SwiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GLId = table.Column<Int32>(type: "smallint", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Bank", x => x.BankId);
                });

            migrationBuilder.CreateTable(
                name: "M_Barge",
                columns: table => new
                {
                    BargeId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    BargeICode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BargeIName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallSign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMOCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GRT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LicenseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BargeIType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Flag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Category", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_ChartOfAccount",
                columns: table => new
                {
                    GLId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GLCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GLName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccTypeId = table.Column<Int16>(type: "tinyint", nullable: false),
                    AccGroupId = table.Column<Int16>(type: "tinyint", nullable: false),
                    COACategoryId1 = table.Column<Int16>(type: "tinyint", nullable: false),
                    COACategoryId2 = table.Column<Int16>(type: "tinyint", nullable: false),
                    COACategoryId3 = table.Column<Int16>(type: "tinyint", nullable: false),
                    IsSysControl = table.Column<bool>(type: "bit", nullable: false),
                    SeqNo = table.Column<Int32>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_ChartOfAccount", x => x.GLId);
                });

            migrationBuilder.CreateTable(
                name: "M_COACategory1",
                columns: table => new
                {
                    COACategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    COACategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COACategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<Int32>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_COACategory1", x => x.COACategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_COACategory2",
                columns: table => new
                {
                    COACategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    COACategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COACategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<Int32>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_COACategory2", x => x.COACategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_COACategory3",
                columns: table => new
                {
                    COACategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    COACategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COACategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeqNo = table.Column<Int32>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_COACategory3", x => x.COACategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_Country",
                columns: table => new
                {
                    CountryId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: true),
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
                    CreditTermId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CreditTermCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditTermName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoDays = table.Column<Int32>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CreditTerm", x => x.CreditTermId);
                });

            migrationBuilder.CreateTable(
                name: "M_CreditTermDt",
                columns: table => new
                {
                    CreditTermId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    FromDay = table.Column<Int16>(type: "tinyint", nullable: false),
                    ToDay = table.Column<Int16>(type: "tinyint", nullable: false),
                    IsEndOfMonth = table.Column<bool>(type: "bit", nullable: false),
                    DueDay = table.Column<Int16>(type: "tinyint", nullable: false),
                    NoMonth = table.Column<Int16>(type: "tinyint", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_Currency",
                columns: table => new
                {
                    CurrencyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsMultiply = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Currency", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "M_CurrencyDt",
                columns: table => new
                {
                    CurrencyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    ExhRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_CurrencyLocalDt",
                columns: table => new
                {
                    CurrencyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    ExhRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_CustomeGroupCreditLimt",
                columns: table => new
                {
                    GroupCreditLimitId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GroupCreditLimitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupCreditLimitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_CustomeGroupCreditLimt", x => x.GroupCreditLimitId);
                });

            migrationBuilder.CreateTable(
                name: "M_Customer",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CustomerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerOtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerRegNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CreditTermId = table.Column<Int16>(type: "tinyint", nullable: false),
                    ParentCustomerId = table.Column<int>(type: "int", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    IsVendor = table.Column<bool>(type: "bit", nullable: false),
                    IsTrader = table.Column<bool>(type: "bit", nullable: false),
                    IsSupplier = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    AddressId = table.Column<Int32>(type: "smallint", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<Int32>(type: "smallint", nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefaultAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleveryAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsFinAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsSalesAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_CustomerContact",
                columns: table => new
                {
                    ContactId = table.Column<Int32>(type: "smallint", nullable: false),
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
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_CustomerCreditLimit",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    EffectFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsExpires = table.Column<bool>(type: "bit", nullable: false),
                    CreditLimitAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_Department",
                columns: table => new
                {
                    DepartmentId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    DepartmentCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Department", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "M_Designation",
                columns: table => new
                {
                    DesignationId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    DesignationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DesignationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Designation", x => x.DesignationId);
                });

            migrationBuilder.CreateTable(
                name: "M_Employee",
                columns: table => new
                {
                    EmployeeId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    EmployeeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeOtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeePhoto = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeSignature = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DepartmentId = table.Column<Int32>(type: "smallint", nullable: false),
                    EmployeeSex = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MartialStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeDOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeJoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeLastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeOffEmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmployeeOtherEmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Employee", x => x.EmployeeId);
                });

            migrationBuilder.CreateTable(
                name: "M_GroupCreditLimt",
                columns: table => new
                {
                    GroupCreditLimitId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GroupCreditLimitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupCreditLimitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_GroupCreditLimt", x => x.GroupCreditLimitId);
                });

            migrationBuilder.CreateTable(
                name: "M_GroupCreditLimt_Customer",
                columns: table => new
                {
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GroupCreditLimitId = table.Column<Int32>(type: "smallint", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_GroupCreditLimtDt",
                columns: table => new
                {
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GroupCreditLimitId = table.Column<Int32>(type: "smallint", nullable: false),
                    EffectFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsExpires = table.Column<bool>(type: "bit", nullable: false),
                    CreditLimitAmt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_Gst",
                columns: table => new
                {
                    GstId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GstCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GstCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Gst", x => x.GstId);
                });

            migrationBuilder.CreateTable(
                name: "M_GstCategory",
                columns: table => new
                {
                    GstCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GstCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GstCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_GstCategory", x => x.GstCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_GstDt",
                columns: table => new
                {
                    GstId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    GstPercentahge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_OrderType",
                columns: table => new
                {
                    OrderTypeId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    OrderTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderTypeCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_OrderType", x => x.OrderTypeId);
                });

            migrationBuilder.CreateTable(
                name: "M_OrderTypeCategory",
                columns: table => new
                {
                    OrderTypeCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    OrderTypeCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderTypeCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_OrderTypeCategory", x => x.OrderTypeCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_PaymentType",
                columns: table => new
                {
                    PaymentTypeId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    PaymentTypeCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_PaymentType", x => x.PaymentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "M_Port",
                columns: table => new
                {
                    PortId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    PortRegionId = table.Column<Int32>(type: "smallint", nullable: false),
                    PortCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Port", x => x.PortId);
                });

            migrationBuilder.CreateTable(
                name: "M_PortRegion",
                columns: table => new
                {
                    PortRegionId = table.Column<Int32>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    PortRegionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PortRegionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<Int32>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditBy = table.Column<Int32>(type: "smallint", nullable: true),
                    EditDateId = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    SubCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    SupplierCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierOtherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierRegNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CreditTermId = table.Column<Int16>(type: "tinyint", nullable: false),
                    ParentSupplierId = table.Column<int>(type: "int", nullable: false),
                    IsCustomer = table.Column<bool>(type: "bit", nullable: false),
                    IsVendor = table.Column<bool>(type: "bit", nullable: false),
                    IsTrader = table.Column<bool>(type: "bit", nullable: false),
                    IsSupplier = table.Column<bool>(type: "bit", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                    AddressId = table.Column<Int32>(type: "smallint", nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PinCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryId = table.Column<Int32>(type: "smallint", nullable: false),
                    PhoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FaxNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailAdd = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WebUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefaultAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsDeliveryAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsFinAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsSalesAdd = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_SupplierContact",
                columns: table => new
                {
                    ContactId = table.Column<Int32>(type: "smallint", nullable: false),
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
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_Tax",
                columns: table => new
                {
                    TaxId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    TaxCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Tax", x => x.TaxId);
                });

            migrationBuilder.CreateTable(
                name: "M_TaxCategory",
                columns: table => new
                {
                    TaxCategoryId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    TaxCategoryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxCategoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_TaxCategory", x => x.TaxCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "M_TaxDt",
                columns: table => new
                {
                    TaxId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    TaxPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_Uom",
                columns: table => new
                {
                    UomId = table.Column<Int16>(type: "tinyint", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    UomCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Uom", x => x.UomId);
                });

            migrationBuilder.CreateTable(
                name: "M_UomDt",
                columns: table => new
                {
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    UomId = table.Column<Int16>(type: "tinyint", nullable: false),
                    PackUomId = table.Column<Int16>(type: "tinyint", nullable: false),
                    UomFactor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_Vessel",
                columns: table => new
                {
                    VesselId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
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
                    CreateById = table.Column<Int32>(type: "smallint", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<Int32>(type: "smallint", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Vessel", x => x.VesselId);
                });

            migrationBuilder.CreateTable(
                name: "M_Vessel_Back",
                columns: table => new
                {
                    VesselId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
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
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "M_Voyage",
                columns: table => new
                {
                    VoyageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<Int16>(type: "tinyint", nullable: false),
                    VoyageNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselId = table.Column<int>(type: "int", nullable: false),
                    BargeId = table.Column<Int32>(type: "smallint", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreateById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_M_Voyage", x => x.VoyageId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "M_AccountSetup");

            migrationBuilder.DropTable(
                name: "M_AccountSetupCategory");

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
                name: "M_CustomeGroupCreditLimt");

            migrationBuilder.DropTable(
                name: "M_Customer");

            migrationBuilder.DropTable(
                name: "M_CustomerAddress");

            migrationBuilder.DropTable(
                name: "M_CustomerContact");

            migrationBuilder.DropTable(
                name: "M_CustomerCreditLimit");

            migrationBuilder.DropTable(
                name: "M_Department");

            migrationBuilder.DropTable(
                name: "M_Designation");

            migrationBuilder.DropTable(
                name: "M_Employee");

            migrationBuilder.DropTable(
                name: "M_GroupCreditLimt");

            migrationBuilder.DropTable(
                name: "M_GroupCreditLimt_Customer");

            migrationBuilder.DropTable(
                name: "M_GroupCreditLimtDt");

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
                name: "M_Vessel_Back");

            migrationBuilder.DropTable(
                name: "M_Voyage");
        }
    }
}