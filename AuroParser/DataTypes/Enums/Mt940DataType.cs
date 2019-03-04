using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroParser.DataTypes.Enums
{
    public enum Mt940DataType
    {
        /* CustomerStatementMessage */
        // Used by the Sender to unambiguously identify the message
        TransactionReference,
        // If the MT 940 is sent in response to an MT920 Request Message, this field must contain the field 20 Transaction Reference Number of the request message
        RelatedMessage,
        // This field identifies the account for which the statement is sent
        Account,
        // Sequential number of the statement, optionally followed by the sequence number of the message within that statement when more than one message is sent for one statement
        StatementNumber,
        // Indicating for the (intermediate) opening balance, whether it is a debit or credit balance, the date, the currency and the amount of the balance
        OpeningBalance,
        OpeningBalanceAmount,
        OpeningBalanceCurrency,
        OpeningBalanceDebitCreditMark,
        OpeningBalanceEntryDate,
        // Indicating for the (intermediate) closing balance, whether it is a debit or credit balance, the date, the currency and the amount of the balance
        ClosingBalance,
        ClosingBalanceAmount,
        ClosingBalanceCurrency,
        ClosingBalanceDebitCreditMark,
        ClosingBalanceEntryDate,
        // Indicates the funds which are available to the account owner (if credit balance) or the balance which is subject to interest charges (if debit balance)
        ClosingAvailableBalance,
        ClosingAvailableBalanceAmount,
        ClosingAvailableBalanceCurrency,
        ClosingAvailableBalanceDebitCreditMark,
        ClosingAvailableBalanceEntryDate,
        // Indicates the funds which are available to the account owner (if a credit or debit balance) for the specified forward value date
        ForwardAvailableBalance,
        ForwardAvailableBalanceAmount,
        ForwardAvailableBalanceCurrency,
        ForwardAvailableBalanceDebitCreditMark,
        ForwardAvailableBalanceEntryDate,
        /* Transaction */
        // Information about the transaction
        TransactionStatementLine,
        // Additional information about the transaction detailed in the preceding statement line and which is to be passed on to the account owner
        TransactionDescription,

        /* TransactionStatementLine parts*/
        // Value date of the transaction
        ValueDate,
        // Entry date of the transaction
        EntryDate,
        // Debit/Credit Mark
        DebitCreditMark,
        // 3rd character of the currency code, if needed
        FundsCode,
        // Amount of the transaction
        Amount,
        // Transaction Type Identification Code
        TransactionType,
        // Customer reference
        CustomerReference,
        // Bank reference
        BankReference,
        // Subfield supplementary details
        SupplementaryDetails
    }
}
