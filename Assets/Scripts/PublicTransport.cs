using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    public sealed class PublicTransport : MonoBehaviour
    {
        public int Level { get; set; } = 1;
        private readonly int maxLevel = 5;
        public City city;
        public TMPro.TMP_Text publicTransportLevelLabel;
        public void OnUpgradePublicTransportClick()
        {
            if (Level < maxLevel)
            {
                city.financeManager.PublicTransportUpgrade();
                Level++;
            }
        }
        private void Update()
        {
            publicTransportLevelLabel.text = Level.ToString();
        }
    }
}
