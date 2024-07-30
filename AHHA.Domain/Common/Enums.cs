﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AHHA.Core.Common
{
    public enum Mode
    {
        Create = 1,
        Update = 2,
        Delete = 3,
    }

    public enum Modules
    {
        Master = 1,
        Sales = 2,
        Purchase = 3,
        AR = 25,
        AP = 26,
        CB = 27,
        GL = 28,
        Admin = 100,
    }

    public enum Master
    {
        Country = 1,
        Currency = 2,
        Department = 3,
        COACategory1 = 5,
        COACategory2 = 6,
        COACategory3 = 7,
        ChartOfAccount = 8,
        CreditTerms = 9,
        Uom = 10,
        Employee = 11,
        Bank = 12,
        Designation = 13,
        Port = 14,
        PortRegion = 15,
        Tax = 16,
        TaxCategory = 17,
        Barge = 18,
        Vessel = 19,
        OrderType = 20,
        OrderTypeCategory = 21,
        CreditTermDt = 22,
        CurrencyDt = 23,
        CurrencyLocalDt = 24,
        Voyage = 25,
        Customer = 26,
        Supplier = 27,
        Gst = 28,
        GstCategory = 29,
        AccountSetup = 30,
        Product = 31,
        UomDt = 32,
        GstDt = 33,
        TaxDt = 34,
        Category = 35,
        SubCategory = 36,
        GroupCreditLimt = 37,
        CustomerGroupCreditLimt = 38,
        AccountSetupCategory = 39,
        GroupCreditLimtDt = 40,
        SupplierContact = 41,
        AccountType = 42,
        AccountGroup = 43,
        CustomerCreditLimit = 44,
        GroupCreditLimt_Customer = 45,
    }

    public enum AR
    {
        Invoice = 1,
        DebitNote = 2,
        CreditNote = 3,
        Adjustment = 4,
        Receipt = 5,
        Refund = 6,
        DocSetoff = 7,
        Reports = 99,
    }

    public enum AP
    {
        Invoice = 1,
        DebitNote = 2,
        CreditNote = 3,
        Adjustment = 4,
        Receipt = 5,
        Refund = 6,
        DocSetoff = 7,
        Reports = 99,
    }

    public enum CB
    {
        CBReceipt = 1,
        CBPayment = 2,
        CBPattyCash = 3,
        CBBankTransfer = 4,
        CBBankRecon = 5,
        Reports = 99,
    }

    public enum GL
    {
        JournalEntry = 1,
        ArApContra = 2,
        FixedAsset = 3,
        OpenBalance = 4,
        YearEndProcess = 5,
        PeriodClose = 6,
        Reports = 99,
    }

    public enum Admin
    {
        User = 1,
        UserRights = 2,
        UserGroup = 3,
        UserGroupRights = 4,
        DocumentNo = 5,
    }
}