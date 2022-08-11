using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Simcity
{
    public sealed class FinanceManager : MonoBehaviour
    {
        public Game game;
        public TMPro.TMP_InputField taxRateInputField;
        public TMPro.TMP_Text balanceLabel;
        private readonly float pricePerSingleRoadMaintenance = 100;
        public float TaxRatePercentage { get; private set; }
        /// <summary>
        /// used for formatting currency output
        /// </summary>
        private readonly System.Globalization.CultureInfo cultureInfo;
        /// <summary>
        /// backing field for Balance
        /// </summary>
        private float balance;
        /// <summary>
        /// lock for Balance
        /// </summary>
        private readonly object balanceLock;
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
                if (balance < 0)
                {
                    game.isGameLost = true;
                }
            }
        }

        private FinanceManager()
        {
            cultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-us");
            balanceLock = new object();
            // start balance
            Balance = 10000;
            // start tax rate
            TaxRatePercentage = 20;
        }

        private void Start()
        {
            taxRateInputField.text = TaxRatePercentage.ToString();

            StartCoroutine(RoadMaintenance());
        }

        private void Update()
        {
            balanceLabel.text = balance.ToString("C2", cultureInfo);
        }

        private IEnumerator RoadMaintenance()
        {
            while (true)
            {
                float priceForRoadMaintenance = 0;
                for (int x = 0; x < game.city.map.blocks.GetLength(0); x++)
                {
                    for (int y = 0; y < game.city.map.blocks.GetLength(1); y++)
                    {
                        var currBlock = game.city.map.blocks[x, y];
                        if (currBlock is MapNamespace.RoadBlock)
                        {
                            priceForRoadMaintenance += (currBlock as MapNamespace.RoadBlock).Level * pricePerSingleRoadMaintenance;
                        }
                    }
                }
                Debug.Log($"Paying for road maintenance (${priceForRoadMaintenance})");
                if (priceForRoadMaintenance > 0)
                {
                    BalanceChange(-priceForRoadMaintenance);
                }
                // wait one day
                yield return new WaitForSeconds(60 * 24);
            }
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

        public void BlockBuildPayment()
        {
            lock (balanceLock)
            {
                Balance -= 1000;
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

        public void PublicTransportUpgrade()
        {
            lock (balanceLock)
            {
                Balance -= 1000;
            }
        }

        /// <summary>
        /// carry out a transaction given amount
        /// </summary>
        /// <param name="amount">if amount is positive, the money will be added to balance. if amount is negative, the money will be deducted from balance</param>
        public void BalanceChange(float amount)
        {
            lock (balanceLock)
            {
                Balance += amount;
            }
        }

        public void RoadUpgrade()
        {
            lock (balanceLock)
            {
                Balance -= 1000;
            }
        }

        public void LoadFromFinanceManagerData(SaveSystem.GameData.CityData.FinanceManagerData financeManagerData)
        {
            Balance = financeManagerData.balance;
            TaxRatePercentage = financeManagerData.taxRatePercentage;
            taxRateInputField.text = TaxRatePercentage.ToString();
        }
    }
}
