using Simcity.MapNamespace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simcity
{
    public abstract class Person
    {
        protected City City { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; set; }

        /// <summary>
        /// Reference to the block where the person
        /// currently is
        /// </summary>
        public MapBlock CurrentBlock { get; protected set; }

        protected Person(string firstName, string lastName, int age, City city)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            City = city;
        }

        public abstract void SimulateOneStep();

        /// <summary>
        /// teleport home/start position of tourist and restart behavior algorithm
        /// </summary>
        public abstract void ResetSimulation();
        protected IEnumerable GetShoppingSimulator(MapBlock returnToBlock)
        {
            // WARNING: this runs in parallel
            var availableShops = City.map.GetAvailableShops();

            if (availableShops.Count == 0) yield break;

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

                bool successfulyStartedShopping = false;
                lock (shop.shoppersLock)
                {
                    if (shop.Shoppers.Count < shop.ShoppersCapacity)
                    {
                        shop.Shoppers.Add(this);
                        successfulyStartedShopping = true;
                    }
                }

                if (successfulyStartedShopping)
                {
                    Debug.Log($"[{FirstName} {LastName}] Started shopping");

                    // spend 20 minutes in the shop
                    for (int i = 0; i < 20; i++)
                    {
                        yield return null;
                    }

                    City.financeManager.ShopPayment();
                    lock (shop.shoppersLock)
                    {
                        shop.Shoppers.Remove(this);
                    }

                    Debug.Log($"[{FirstName} {LastName}] Finished shopping");
                }
                // else go home, the shop is full
            }


            // go home
            if (CurrentBlock != returnToBlock)
            {
                Debug.Log($"[{FirstName} {LastName}] is starting journey home");

                var goHomeSimulator = SimulateGoTo(returnToBlock);
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
                        var teleportStatus = MoveTo(returnToBlock);
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
        protected IEnumerable<bool> SimulateGoTo(MapBlock destination)
        {
            // returns the path or null if the destination is unreachable
            List<MapBlock> ComputePath()
            {
                var previous = new MapBlock[City.map.GridSize, City.map.GridSize];
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
                            var upperAdjacentBlock = City.map.blocks[currBlock.Coordinates.x, currBlock.Coordinates.y - 1];
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
                        if (currBlock.Coordinates.x + 1 < City.map.blocks.GetLength(0))
                        {
                            var rightAdjacentBlock = City.map.blocks[currBlock.Coordinates.x + 1, currBlock.Coordinates.y];
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
                        if (currBlock.Coordinates.y + 1 < City.map.blocks.GetLength(1))
                        {
                            var lowerAdjacentBlock = City.map.blocks[currBlock.Coordinates.x, currBlock.Coordinates.y + 1];
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
                            var leftAdjacentBlock = City.map.blocks[currBlock.Coordinates.x - 1, currBlock.Coordinates.y];
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
                    if (City.map.blocks[block.Coordinates.x, block.Coordinates.y] == block)
                    {
                        var success = MoveTo(block);
                        if (success)
                        {
                            Debug.Log($"[{FirstName} {LastName}] moved to {block.Coordinates.x}, {block.Coordinates.y}");
                            yield return true;
                        }
                        else
                        {
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
        /// <returns>true of the move succeeded or false if the move failed (there
        /// were too many people on the destination block)</returns>
        protected bool MoveTo(MapBlock destBlock)
        {
            if (destBlock != CurrentBlock)
            {
                // we need to lock both the destination and current block
                // we prevent deadlocks by locking in a specified order
                object firstLock, secondLock;
                {
                    if (destBlock.Coordinates.y < CurrentBlock.Coordinates.y)
                    {
                        firstLock = destBlock.peopleHereLock;
                        secondLock = CurrentBlock.peopleHereLock;
                    }
                    else if (destBlock.Coordinates.y > CurrentBlock.Coordinates.y)
                    {
                        firstLock = CurrentBlock.peopleHereLock;
                        secondLock = destBlock.peopleHereLock;
                    }
                    else
                    {
                        if (destBlock.Coordinates.x < CurrentBlock.Coordinates.x)
                        {
                            firstLock = destBlock.peopleHereLock;
                            secondLock = CurrentBlock.peopleHereLock;
                        }
                        else
                        {
                            firstLock = CurrentBlock.peopleHereLock;
                            secondLock = destBlock.peopleHereLock;
                        }
                    }
                }
                lock (firstLock)
                {
                    lock (secondLock)
                    {
                        if (destBlock.PeopleHere.Count < destBlock.PeopleHereCapacity)
                        {

                            CurrentBlock.PeopleHere.Remove(this);
                            destBlock.PeopleHere.Add(this);
                            CurrentBlock = destBlock;
                            Debug.Log($"{FirstName} {LastName} moved to ({destBlock.Coordinates.x}, {destBlock.Coordinates.y})");
                        }
                        else
                        {

                            // the capacity of the block is full
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}
