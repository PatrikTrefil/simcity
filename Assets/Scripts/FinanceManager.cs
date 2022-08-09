using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    public sealed class FinanceManager : MonoBehaviour
    {
        public TMPro.TMP_Text balanceLabel;
        public float TaxRatePercentage { get; set; }
        /// <summary>
        /// used for formatting currency output
        /// </summary>
        private System.Globalization.CultureInfo cultureInfo;
        /// <summary>
        /// backing field for Balance
        /// </summary>
        private float balance;
        /// <summary>
        /// lock for Balance
        /// </summary>
        private object balanceLock;
        /// <summary>
        /// lock balanceLock if accessing from multiple threads
        /// </summary>
        public float Balance
        {
            get
            {
                return balance;
            }
            private set
            {
                balance = value;
                balanceLabel.text = balance.ToString("C2", cultureInfo);
            }
        }

        private FinanceManager()
        {
            cultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-us");
            balanceLock = new object();
        }

        private void Awake()
        {
            // start balance
            Balance = 10000;
            // start tax rate
            TaxRatePercentage = 20;
        }

        public void ShopPayment()
        {
            lock (balanceLock)
            {
                Balance += TaxRatePercentage * 10;
            }
        }

        public void WagePayout(float wagePaidOut)
        {
            lock (balanceLock)
            {
                Balance += wagePaidOut * (TaxRatePercentage / 100);
            }
        }

        public void OnTaxRateEndEdit(TMPro.TMP_InputField inputField)
        {
            try
            {
                TaxRatePercentage = float.Parse(inputField.text);
            }
            catch (FormatException)
            {
                inputField.text = TaxRatePercentage.ToString();
            }
            Debug.Log($"TaxRatePercentage set to: {TaxRatePercentage} %");
        }
    }
}
