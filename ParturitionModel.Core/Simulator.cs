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
        private List<Person> _persons;

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

        private Task InitAsync(CancellationToken ct = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                _environment = new Environment();
                _persons = new List<Person>();

                _populationFactor = Math.Pow(2.0, 1.0/_settings.DoublingPopulationTime);

                for (int i = 0; i < _settings.PopulationSize; ++i)
                {
                    var person = CreateNewPerson();
                    _persons.Add(person);

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
                BirthYear = -_rnd.Next(0, _settings.LifeLimit + 1)
            };

            return person;
        }

        private async Task RunAsync(CancellationToken ct = default(CancellationToken))
        {
            for (int i = 0; i < _settings.SimulationTime; ++i)
            {
                ct.ThrowIfCancellationRequested();
                await NextStepAsync(ct);
            }
        }

        private async Task NextStepAsync(CancellationToken ct = default(CancellationToken))
        {
            SimulationEventArgs args = new SimulationEventArgs();
            args.CurrentYear = _environment.Year;

            //remove old people
            int index = 0;
            while (index < _persons.Count)
            {
                var person = _persons[index];
                if (person.Age >= _settings.LifeLimit)
                {
                    _persons.RemoveAt(index);
                    ++args.NaturalDeathCount;
                }
                else
                {
                    ++index;
                }
            }

            //get all nubility men
            var nubilityMen = _persons.Where(x => x.Sex == Sex.Man && x.Age >= _settings.NubilityManAge).ToList();

            //prego simulation
            foreach (var woman in _persons.Where(x => x.Sex == Sex.Woman).ToList())
            {
                int pregoIndex;
                for (pregoIndex = 0; pregoIndex < _settings.BirthYears.Length; ++pregoIndex)
                {
                    var pregoAge = _settings.BirthYears[pregoIndex];
                    if (woman.Age == pregoAge)
                    {
                        //find nubility man
                        int nubilityManIndex = _rnd.Next(0, nubilityMen.Count);
                        var nubilityMan = nubilityMen[nubilityManIndex];
                        var child = await GenerateChild(nubilityMan, woman, _settings.BirthDeathFactor[pregoIndex], ct);

                        if (child == null)
                        {
                            _persons.Remove(woman);
                            ++args.ChildDeath;
                        }
                        else
                        {
                            _persons.Add(child);
                        }
                    }
                }
            }

            //remove extra people
            int peopleCount = (int) Math.Round(_settings.PopulationSize*Math.Pow(_populationFactor, _environment.Year));
            while (_persons.Count > peopleCount)
            {
                var indexForRemove = _rnd.Next(0, _persons.Count);
                _persons.RemoveAt(indexForRemove);
                ++args.ExtraDeath;
            }

            SimulationEvent(this, args);

            _environment.IncreaseYear();
        }

        private Task<Person> GenerateChild(Person father, Person mother, double deathFactor, CancellationToken ct = default(CancellationToken))
        {
            return Task.Run(() =>
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
                parentDeathFactor *= deathFactor;
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
            }, ct);
        }
    }
}