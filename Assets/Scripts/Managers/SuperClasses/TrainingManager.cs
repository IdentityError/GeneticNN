//using System.Collections.Generic;
//using UnityEngine;

//public class TrainingManager : Manager
//{
//    [Header("Training")]
//    public int populationNumber;
//    [Range(0F, 1F)]
//    [SerializeField] private float mutationRate;
//    [SerializeField] private bool saveFittest;
//    [SerializeField] private bool trainFittest;

// protected int generationCount = 0; protected CarIndividual[] population = null; [SerializeField]
// private List<CarIndividual> completedTrackIndividuals = new List<CarIndividual>(); protected int currentAlive;

// private DNA.DnaTopology simulationTopology;

// private TrackBest currentBestSimulation = new TrackBest(); private CarIndividualData chosenOne = null;

// protected void Start() { population = new CarIndividual[populationNumber];

// TSaveObject saveObject =
// TSaveManager.GetInstance().LoadPersistentData(TSaveManager.TRACKS_STATS); if (saveObject != null)
// { TracksStats stats = saveObject.GetData<TracksStats>(); TrackBest best = stats.Get(track.name);
// if (best != null) { currentBestSimulation = best; uiManager.UpdateSavedBestStats(best); } else {
// uiManager.AppendToLog("Unable to find the best stats for this track"); } } else {
// uiManager.AppendToLog("Unable to find Tracks Stats"); }

// if (trainFittest) { saveObject =
// TSaveManager.GetInstance().LoadPersistentData(TSaveManager.CHOSEN_ONE); if (saveObject != null) {
// chosenOne = saveObject.GetData<CarIndividualData>(); DNA bestDna = chosenOne.GetDNA();
// StartNewSimulation(CreatePopulationFromSeedDNA(bestDna)); simulationTopology = bestDna.topology;
// } else { uiManager.AppendToLog("Unable to find the chosen one, starting a clear training
// session..."); simulationTopology =
// carIndividualPrefab.GetComponent<CarIndividual>().predefinedTopology; StartNewSimulation(null); }
// } else { uiManager.AppendToLog("Starting a clear training session..."); simulationTopology =
// carIndividualPrefab.GetComponent<CarIndividual>().predefinedTopology; StartNewSimulation(null); }
// uiManager.DrawNetUI(simulationTopology); uiManager.UpdateTrackLength(CalculateTrackLength()); }

// protected void Update() { Time.timeScale = timeScale; }

// public void SaveCurrentBest() { if (completedTrackIndividuals.Count > 0) {
// TSaveManager.GetInstance().SavePersistentData(completedTrackIndividuals[0].GetIndividualData(),
// TSaveManager.CHOSEN_ONE); uiManager.AppendToLog("Saved a new chosen one: " +
// completedTrackIndividuals[0].name); } else { uiManager.AppendToLog("No individual has completed
// the track yet!"); } }

// public void CalculateFitness(CarIndividual individual) { individual.fitness += (2F *
// individual.stats.averageThrottle + Vector3.Distance(individual.transform.position,
// individual.lastPosition)) * Time.deltaTime; //individual.fitness = 6F * (float)Math.Exp(1.5F *
// (individual.stats.averageThrottle - 1F)) * (2F * individual.stats.distance) * Time.deltaTime; }

// public override void HasCompletedSimulation(CarIndividual individual) { if (individual != null) {
// if (individual.stats.BetterThan(currentBestSimulation.stats)) { currentBestSimulation.stats =
// individual.stats; TrackBest trackBest = new TrackBest(individual.stats,
// individual.GetIndividualData()); if (saveFittest) { uiManager.AppendToLog("New best on this
// track, overriding it..."); TSaveObject saveObj =
// TSaveManager.GetInstance().LoadPersistentData(TSaveManager.TRACKS_STATS); if (saveObj != null) {
// TracksStats tracksStats = saveObj.GetData<TracksStats>(); tracksStats.AddOrReplace(trackBest);
// TSaveManager.GetInstance().SavePersistentData(tracksStats, TSaveManager.TRACKS_STATS); } else {
// TracksStats newStats = new TracksStats(); newStats.AddOrReplace(trackBest);
// TSaveManager.GetInstance().SavePersistentData(newStats, TSaveManager.TRACKS_STATS);
// uiManager.AppendToLog("TrackStats not found, creating new one and setting the best for this
// track..."); } } uiManager.UpdateSavedBestStats(trackBest); } }

// if (individual != null) { completedTrackIndividuals.Add(individual); } }

// protected override CarIndividual InstantiateAndInitializeIndividual(DNA dna, string name) {
// CarIndividual newCar = base.InstantiateAndInitializeIndividual(dna, name); newCar.manager = this;
// return newCar; }

// protected void StartNewSimulation(CarIndividual[] newPopulation) { if (newPopulation == null) {
// for (int i = 0; i < populationNumber; i++) { population[i] =
// InstantiateAndInitializeIndividual(null, "Car" + i); } return; }

// currentAlive = populationNumber; generationCount++;

// uiManager.UpdateGenerationCount(generationCount);

// for (int i = 0; i < populationNumber; i++) { if (population[i] != null) {
// Destroy(population[i].gameObject); } newPopulation[i].gameObject.SetActive(true); population[i] =
// newPopulation[i]; } }

// public void EndAndRestartNewSimulation() { for (int i = 0; i < populationNumber; i++) {
// population[i].CompletedTrack(false); }

// completedTrackIndividuals.Clear(); currentAlive = 0; CarIndividual[] newPopulation =
// CrossoverPopulation(); StartNewSimulation(newPopulation); }

// private CarIndividual GetFittest() { float maxFitness = 0; CarIndividual best = null; for (int i
// = 0; i < populationNumber; i++) { if (population[i].fitness > maxFitness) { maxFitness =
// population[i].fitness; best = population[i]; } } return best; }

// protected CarIndividual[] CrossoverPopulation() { //NormalizePopulationBreedingProbabilities();
// CarIndividual[] newPopulation = new CarIndividual[populationNumber];

// CarIndividualData[] parents = PickFittestTwo();

// for (int i = 0; i < populationNumber; i++) { DNA childDna =
// parents[0].GetDNA().Crossover(parents[1].GetDNA(), 3); childDna.Mutate(0, mutationRate);

// CarIndividual child = InstantiateAndInitializeIndividual(childDna, "Car" + i);
// child.gameObject.SetActive(false); newPopulation[i] = child; } return newPopulation; }

// protected CarIndividualData[] PickFittestTwo() { CarIndividualData[] returnOut = new
// CarIndividualData[2]; CarIndividual firstCar = null; float firstFitness = 0F; foreach
// (CarIndividual car in population) { if (car.fitness > firstFitness) { firstCar = car;
// firstFitness = car.fitness; } } float secondFitness = 0F; CarIndividual secondCar = null; foreach
// (CarIndividual car in population) { if (car.fitness > secondFitness) { if (populationNumber > 1)
// { if (car != firstCar) { secondCar = car; secondFitness = car.fitness; } } else { secondCar =
// car; secondFitness = car.fitness; } } } returnOut[0] = new
// CarIndividualData(firstCar.neuralNet.dna, firstFitness); returnOut[1] = new
// CarIndividualData(secondCar.neuralNet.dna, secondFitness); return returnOut; }

// protected void NormalizePopulationBreedingProbabilities() { float fitnessSum = 0; foreach
// (CarIndividual car in population) { fitnessSum += car.fitness; }

// foreach (CarIndividual car in population) { car.breedingProbability = car.fitness / fitnessSum; } }

// protected CarIndividual PickRandom() { float seed = UnityEngine.Random.Range(0F, 1F); int index =
// -1; while (seed >= 0) { seed -= population[++index].breedingProbability; } return
// population[index]; }

//    protected CarIndividual[] CreatePopulationFromSeedDNA(DNA seedDna)
//    {
//        CarIndividual[] newPopulation = new CarIndividual[populationNumber];
//        for (int i = 0; i < populationNumber; i++)
//        {
//            DNA individualDna = new DNA(seedDna.topology, seedDna.weights);
//            individualDna.Mutate(0, mutationRate);
//            newPopulation[i] = InstantiateAndInitializeIndividual(individualDna, "Car" + i);
//        }
//        return newPopulation;
//    }
//}