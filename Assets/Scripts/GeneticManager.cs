using System.Collections.Generic;
using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    [Header("References")]
    public GameObject carIndividualPrefab;

    public int populationNumber;
    public float mutationPercentage;

    private List<GameObject> population = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < populationNumber; i++)
        {
            GameObject individual = Instantiate(carIndividualPrefab, new Vector3(0F, 1F, 0F), Quaternion.identity);
            population.Add(individual);
        }
    }
}