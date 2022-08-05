using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEngine;

public class Population : MonoBehaviour
{
    public int startPopulationCount = 1000;
    public TMP_Text populationCountLabel;
    public ObservableCollection<CityResident> People { get; }

    public Population()
    {
        People = new ObservableCollection<CityResident>();
    }
    // Start is called before the first frame update
    void Start()
    {
        populationCountLabel.text = People.Count.ToString();

        People.CollectionChanged += OnPeopleChange;

        AddRandomPeopleToPopulation(startPopulationCount);
    }

    // Update is called once per frame
    void Update()
    {
    }
    private void AddRandomPeopleToPopulation(int count)
    {
        for (int i = 0; i < count; i++)
        {
            People.Add(CityResident.GenerateRandomCityResident());
        }
    }
    /// <summary>
    /// Updates population count on actions which modify count
    /// </summary>
    private void OnPeopleChange(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add ||
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove ||
            e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset
            )
        {
            populationCountLabel.text = People.Count.ToString();
        }
    }
}
