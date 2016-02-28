using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParturitionModel.Core
{
    public sealed class Simulator
    {
        private readonly object _sync = new object();
        private bool _simulationStarted;

        private readonly Settings _settings = new Settings();
        private Environment _environment;
        private Person[] _persons;

        private readonly Random _rnd = new Random();

        private double _populationFactor;

        public event EventHandler<SimulationEventArgs> SimulationEvent = delegate { };

        public async Task StartSimulationAsync(CancellationToken ct = default(CancellationToken))
        {
            lock (_sync)
            {
                if (_simulationStarted)
                {
                    throw new InvalidOperationException();
                }

                _simulationStarted = true;
            }

            await InitAsync(ct);
            await RunAsync(ct);

            lock (_sync)
            {
                _simulationStarted = false;
            }
        }

        public int CurrentPopuation
        {
            get { return _persons.Length; }
        }

        private Task InitAsync(CancellationToken ct = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                _environment = new Environment();
                _persons = new Person[_settings.PopulationSize];

                _populationFactor = Math.Pow(2.0, 1.0/_settings.DoublingPopulationTime);

                for (int i = 0; i < _settings.PopulationSize; ++i)
                {
                    var person = CreateNewPerson();
                    //_persons[i] = person;
                    PutPerson(person, _persons);

                    if (i%1000 == 0)
                    {
                        Console.WriteLine("Added person #{0}", i);
                    }
                }
            }, ct);
        }

        private Person CreateNewPerson()
        {
            var sexFactor = _rnd.NextDouble();
            var deathFactor = _rnd.NextDouble() * _settings.DeathProbability;

            var person = new Person(_environment)
            {
                Sex = sexFactor < _settings.SexFactor ? Sex.Man : Sex.Woman,
                DeathFactor = deathFactor,
                BirthYear = -_rnd.Next(0, _settings.LifeLimit)
            };

            return person;
        }

        private void PutPerson(Person person, Person[] persons)
        {
            var length = persons.Length;
            var index = _rnd.Next(0, length);
            int i = index;
            int j = index - 1;
            while (i < length || j > 0)
            {
                if (i < length && persons[i] == null)
                {
                    persons[i] = person;
                    return;
                }
                ++i;
                if (j >= 0 && persons[j] == null)
                {
                    persons[j] = person;
                    return;
                }
                --j;
            }

            throw new Exception("No space for person in population.");
        }

        private async Task RunAsync(CancellationToken ct = default(CancellationToken))
        {
            for (int i = 0; i < _settings.SimulationTime; ++i)
            {
                await NextStepAsync(ct);
            }
        }

        private Task NextStepAsync(CancellationToken ct = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                SimulationEventArgs args = new SimulationEventArgs();

                _environment.IncreaseYear();
                args.CurrentYear = _environment.Year;

                //new population size
                int peopleCount = (int)Math.Round(_settings.PopulationSize * Math.Pow(_populationFactor, _environment.Year));
                var newPopulation = new Person[peopleCount];

                //main loop
                int currentPopulationSize = 0;
                foreach (var person in _persons)
                {
                    ct.ThrowIfCancellationRequested();

                    ++currentPopulationSize;
                    if (currentPopulationSize > peopleCount)
                    {
                        break;
                    }

                    if (person == null)
                    {
                        continue;
                    }

                    Person child;
                    if (!Process(person, args, out child))
                    {
                        continue;
                    }

                    PutPerson(person, newPopulation);

                    if (child != null)
                    {
                        ++currentPopulationSize;
                        if (currentPopulationSize > peopleCount)
                        {
                            break;
                        }

                        PutPerson(child, newPopulation);
                    }
                }

                _persons = newPopulation;

                SimulationEvent(this, args);
            }, ct);
        }

        private Person SearchNubilityMan()
        {
            while (true)
            {
                int index = _rnd.Next(0, _persons.Length);
                var person = _persons[index];
                if (person != null && person.Sex == Sex.Man && person.Age >= _settings.NubilityManAge)
                {
                    return person;
                }
            }
        }

        private bool Process(Person person, SimulationEventArgs args, out Person child)
        {
            child = null;

            //if too old
            if (person.Age >= _settings.LifeLimit)
            {
                ++args.NaturalDeathCount;
                return false;
            }

            //if man
            if (person.Sex == Sex.Man)
            {
                return true;
            }

            //if not pregnant
            var pregoInfo = _settings.GetPregoInfo(person.Age);
            if (pregoInfo == null)
            {
                return true;
            }

            //get father
            var father = SearchNubilityMan();

            //get child
            BornInfo bornInfo;
            if (!args.BornInfos.TryGetValue(pregoInfo.Order, out bornInfo))
            {
                bornInfo = new BornInfo
                {
                    Order = pregoInfo.Order,
                    MotherAge = person.Age
                };
                args.BornInfos.Add(bornInfo.Order, bornInfo);
            }
            child = GenerateChild(father, person, pregoInfo.Factor);

            if (child == null)
            {
                ++bornInfo.DeathCount;
                return false;
            }

            ++bornInfo.BornCount;
            return true;
        }

        private Person GenerateChild(Person father, Person mother, double deathFactor)
        {
            var parentDeathFactor = _rnd.NextDouble() <= 0.5 ? father.DeathFactor : mother.DeathFactor;
            var childDeathFactor = parentDeathFactor + 2.0 * _settings.MutationFactor * _rnd.NextDouble() - _settings.MutationFactor;
            if (childDeathFactor < 0.0)
            {
                childDeathFactor = 0.0;
            }
            else if (childDeathFactor > 1.0)
            {
                childDeathFactor = 1.0;
            }
            parentDeathFactor = mother.DeathFactor * deathFactor;
            var p = _rnd.NextDouble();
            if (p <= parentDeathFactor)
            {
                return null;
            }

            var result = new Person(_environment)
            {
                BirthYear = _environment.Year,
                Sex = _rnd.NextDouble() <= _settings.ChildSexFactor ? Sex.Man : Sex.Woman,
                DeathFactor = childDeathFactor
            };

            return result;
        }
    }
}