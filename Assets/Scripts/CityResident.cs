using Simcity.MapNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    public sealed class CityResident : Person
    {
        /// <summary>
        /// Home
        /// </summary>
        public ResidenceBlock Residence { get; private set; }
        public ShopBlock Workplace { get; private set; }
        /// <summary>
        /// Amount of money obtained for one day of work.
        /// Paid out right after finishing work.
        /// </summary>
        public int DailyWage { get; private set; }
        private City City { get; }
        private IEnumerator simulator;

        public CityResident(string firstName, string lastName, int age, City city, ResidenceBlock residence, ShopBlock workplace) : base(firstName, lastName, age, city.map)
        {
            Residence = residence;
            Workplace = workplace;
            simulator = GetSimulator();
            CurrentBlock = residence;
            City = city;
        }

        public override void SimulateOneStep()
        {
            // WARNING: this runs in parallel
            simulator.MoveNext();
        }

        public override void ResetSimulation()
        {
            var status = MoveTo(Residence);
            if (status == false)
            {
                throw new Exception("Could not teleport home");
            }
            simulator = GetSimulator();
        }

        private IEnumerator GetSimulator()
        {
            while (true)
            {
                // TODO: implement other actions then just going shopping
                var shoppingSimulator = GetShoppingSimulator();
                var shoppingEnumerator = shoppingSimulator.GetEnumerator();
                var notEndReached = shoppingEnumerator.MoveNext();
                while (notEndReached)
                {
                    notEndReached = shoppingEnumerator.MoveNext();
                    yield return null;
                }
                yield return null;
            }
        }

        private IEnumerable GetShoppingSimulator()
        {
            // WARNING: this runs in parallel
            var availableShops = Map.GetAvailableShops();
            ShopBlock randomShop = availableShops[new System.Random().Next(0, availableShops.Count)];

            Debug.Log($"[{FirstName} {LastName}] Decided to go to a shop at {randomShop.Coordinates.x}, {randomShop.Coordinates.y}");

            // go to shop
            var goToShopSimulator = SimulateGoTo(randomShop);
            bool wasJourneyToShopSuccessful = true;
            foreach (var goToShopStatus in goToShopSimulator)
            {
                if (goToShopStatus)
                {
                    Debug.Log($"[{FirstName} {LastName}] is now at {CurrentBlock.Coordinates.x} {CurrentBlock.Coordinates.y}");
                    yield return null;
                }
                else
                {
                    wasJourneyToShopSuccessful = false;
                    Debug.Log($"[{FirstName} {LastName}] journey was disrupted. Going home instead.");
                    break;
                }
            }
            // shopping
            if (wasJourneyToShopSuccessful)
            {
                ShopBlock shop = CurrentBlock as ShopBlock;

                if (shop == null) throw new Exception("Shop is null");

                shop.Shoppers.Add(this);

                Debug.Log($"[{FirstName} {LastName}] Started shopping");

                // spend 60 minutes in the shop
                // HACK: 5
                for (int i = 0; i < 5; i++)
                {
                    yield return null;
                }

                City.financeManager.ShopPayment();
                shop.Shoppers.Remove(this);

                Debug.Log($"[{FirstName} {LastName}] Finished shopping");
            }


            // go home
            if (CurrentBlock != Residence)
            {
                Debug.Log($"[{FirstName} {LastName}] is starting journey home");

                var goHomeSimulator = SimulateGoTo(Residence);
                foreach (var goHomeStatus in goHomeSimulator)
                {
                    if (goHomeStatus)
                    {
                        Debug.Log($"[{FirstName} {LastName}] is now at {CurrentBlock.Coordinates.x} {CurrentBlock.Coordinates.y}");
                        yield return null;
                    }
                    else
                    {
                        // teleport home
                        Debug.Log($"[{FirstName} {LastName}] teleported home");
                        var teleportStatus = MoveTo(Residence);
                        if (teleportStatus == false)
                        {
                            throw new Exception("Could not teleport home");
                        }
                    }
                }
                Debug.Log($"[{FirstName} {LastName}] is now at {CurrentBlock.Coordinates.x} {CurrentBlock.Coordinates.y}");
            }
            else
            {
                Debug.Log($"[{FirstName} {LastName}] decided to stay home instead");
            }
        }

        private IEnumerable<bool> SimulateGoTo(MapBlock destination)
        {
            // returns the path or null if the destination is unreachable
            List<MapBlock> ComputePath()
            {
                var previous = new MapBlock[Map.GridSize, Map.GridSize];
                var queue = new Queue<MapBlock>();

                queue.Enqueue(CurrentBlock);

                while (queue.Count > 0)
                {
                    var currBlock = queue.Dequeue();
                    if (currBlock == destination)
                    {
                        break;
                    }

                    // enqueue all possible movements
                    {
                        // move up
                        if (currBlock.Coordinates.y - 1 >= 0)
                        {
                            var upperAdjacentBlock = Map.blocks[currBlock.Coordinates.x, currBlock.Coordinates.y - 1];
                            // check if the block has been visited
                            if (previous[upperAdjacentBlock.Coordinates.x, upperAdjacentBlock.Coordinates.y] == null)
                            {
                                if (upperAdjacentBlock == destination || upperAdjacentBlock is RoadBlock)
                                {
                                    queue.Enqueue(upperAdjacentBlock);
                                    previous[upperAdjacentBlock.Coordinates.x, upperAdjacentBlock.Coordinates.y] = currBlock;
                                }

                            }
                        }

                        // move right
                        if (currBlock.Coordinates.x + 1 < Map.blocks.GetLength(0))
                        {
                            var rightAdjacentBlock = Map.blocks[currBlock.Coordinates.x + 1, currBlock.Coordinates.y];
                            // check if the block has been visited
                            if (previous[rightAdjacentBlock.Coordinates.x, rightAdjacentBlock.Coordinates.y] == null)
                            {
                                if (rightAdjacentBlock == destination || rightAdjacentBlock is RoadBlock)
                                {
                                    queue.Enqueue(rightAdjacentBlock);
                                    previous[rightAdjacentBlock.Coordinates.x, rightAdjacentBlock.Coordinates.y] = currBlock;
                                }
                            }
                        }

                        // move down
                        if (currBlock.Coordinates.y + 1 < Map.blocks.GetLength(1))
                        {
                            var lowerAdjacentBlock = Map.blocks[currBlock.Coordinates.x, currBlock.Coordinates.y + 1];
                            // check if the block has been visited
                            if (previous[lowerAdjacentBlock.Coordinates.x, lowerAdjacentBlock.Coordinates.y] == null)
                            {
                                if (lowerAdjacentBlock == destination || lowerAdjacentBlock is RoadBlock)
                                {
                                    queue.Enqueue(lowerAdjacentBlock);
                                    previous[lowerAdjacentBlock.Coordinates.x, lowerAdjacentBlock.Coordinates.y] = currBlock;
                                }
                            }
                        }

                        // move left
                        if (currBlock.Coordinates.x - 1 >= 0)
                        {
                            var leftAdjacentBlock = Map.blocks[currBlock.Coordinates.x - 1, currBlock.Coordinates.y];
                            // check if the block has been visited
                            if (previous[leftAdjacentBlock.Coordinates.x, leftAdjacentBlock.Coordinates.y] == null)
                            {
                                if (leftAdjacentBlock == destination || leftAdjacentBlock is RoadBlock)
                                {
                                    queue.Enqueue(leftAdjacentBlock);
                                    previous[leftAdjacentBlock.Coordinates.x, leftAdjacentBlock.Coordinates.y] = currBlock;
                                }
                            }
                        }
                    }
                }
                // reconstruct the path

                // is destination reachable?
                if (previous[destination.Coordinates.x, destination.Coordinates.y] == null)
                {
                    return null;
                }
                List<MapBlock> path;
                {
                    var reversedPath = new List<MapBlock>();
                    var currBlock = destination;
                    // the path will not include CurrentBlock
                    while (currBlock != CurrentBlock)
                    {
                        reversedPath.Add(currBlock);
                        var previousBlock = previous[currBlock.Coordinates.x, currBlock.Coordinates.y];
                        currBlock = previousBlock;
                    }
                    reversedPath.Reverse();
                    path = reversedPath;
                }
                return path;
            }

            var path = ComputePath();
            if (path == null)
            {
                // immediatelly report failure, because the destination
                // block is unreachable
                yield return false;
            }
            else
            {
                // simulate movement
                foreach (var block in path)
                {
                    // assert that the block is still there (map has not changed here)
                    if (Map.blocks[block.Coordinates.x, block.Coordinates.y] == block)
                    {
                        var success = MoveTo(block);
                        if (success)
                        {
                            Debug.Log($"[{FirstName} {LastName}] moved to {block.Coordinates.x}, {block.Coordinates.y}");
                            yield return true;
                        }
                        else
                        {
                            // TODO: implement retries
                            Debug.Log($"[{FirstName} {LastName}] moving to {block.Coordinates.x}, {block.Coordinates.y} failed");
                            yield return false;
                        }
                    }
                    else
                    {
                        yield return false;
                    }
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="map">this will be used to find a residence and workplace</param>
        /// <returns>new resident or null if there is no residence or workplace available</returns>
        public static CityResident GenerateRandomCityResident(City city)
        {
            ResidenceBlock residence;
            ShopBlock workplace;
            {
                var availableResidences = city.map.GetAvailableResidences();
                var availableWorkplaces = city.map.GetAvailableWorkplaces();

                if (availableResidences.Count == 0 || availableWorkplaces.Count == 0) return null;

                residence = availableResidences[UnityEngine.Random.Range(0, availableResidences.Count)];
                workplace = availableWorkplaces[UnityEngine.Random.Range(0, availableWorkplaces.Count)];
            }

            return new CityResident(
                Faker.Name.First(),
                Faker.Name.Last(),
                Faker.RandomNumber.Next(0, 100),
                city,
                residence,
                workplace
            );
        }

        /// <summary>
        /// </summary>
        /// <returns>true of the move succeeded or false if the move failed (there
        /// were too many people on the destination block)</returns>
        private bool MoveTo(MapBlock destBlock)
        {
            if (destBlock != CurrentBlock)
            {
                lock (Map.blockLocks[destBlock.Coordinates.x, destBlock.Coordinates.y])
                {
                    if (destBlock.PeopleHere.Count < destBlock.PeopleHereCapacity)
                    {
                        lock (Map.blockLocks[CurrentBlock.Coordinates.x, CurrentBlock.Coordinates.y])
                        {
                            CurrentBlock.PeopleHere.Remove(this);
                            destBlock.PeopleHere.Add(this);
                            CurrentBlock = destBlock;
                            Debug.Log($"{FirstName} {LastName} moved to ({destBlock.Coordinates.x}, {destBlock.Coordinates.y})");
                        }
                    }
                    else
                    {
                        // the capacity of the block is full
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
