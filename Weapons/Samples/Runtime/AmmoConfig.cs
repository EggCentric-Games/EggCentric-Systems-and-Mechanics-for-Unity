using EggCentric.ValueProviders.PatternGenerators;
using EggCentric.WeaponFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EggCentric.WeaponFramework.Core
{
    public interface IAmmoType
    {
    }
}

namespace EggCentric.WeaponFramework.AmmoProviders
{
    public interface IAmmoProvider<out TAmmo> where TAmmo : IAmmoType
    {
        public TAmmo Peek(int offset = 0);
        public TAmmo GetNext();
    }

    public interface IAmmoSupply<TAmmo> where TAmmo : IAmmoType
    {
        public bool TryGet(TAmmo ammo);
        public bool TryGet(Func<TAmmo, bool> condition, out TAmmo ammo); // to be removed
    }

    public abstract class AmmoContainer<TAmmo> : IAmmoProvider<TAmmo> where TAmmo : IAmmoType
    {
        public IReadOnlyList<TAmmo> Snapshot => _ammoStorage.Snapshot;
        public int RemainingAmmo => _ammoStorage.RemainingAmmo;
        public int Capacity => _capacityPolicy.MaxCapacity(_loadingPattern);

        private IReadOnlyList<TAmmo> _loadingPattern => _loadingPolicy.LoadingPattern != null ? _loadingPolicy.LoadingPattern : _ammoStorage.Snapshot;

        private IAmmoStorage<TAmmo> _ammoStorage;
        private ILoadingPolicy<TAmmo> _loadingPolicy;
        private ICapacityPolicy<TAmmo> _capacityPolicy;

        public TAmmo Peek(int offset = 0) => _ammoStorage.Peek(offset);
        public bool CanAccept(TAmmo ammo) => _loadingPolicy.CanAccept(ammo);
        public bool IsExpected(TAmmo candidate) => _loadingPolicy.IsExpected(candidate);
        public bool CanBeInserted(TAmmo candidate) => _capacityPolicy.CanBeInserted(_ammoStorage.Snapshot, candidate);

        public TAmmo GetNext() => _ammoStorage.Extract();

        public bool TryInsert(TAmmo ammo)
        {
            if(!CanAccept(ammo))
                return false;

            if (!IsExpected(ammo))
                return false;

            if (!CanBeInserted(ammo))
                return false;

            _ammoStorage.Insert(ammo);
            return true;
        }
    }

    public interface IAmmoStorage<TAmmo> where TAmmo : IAmmoType
    {
        public IReadOnlyList<TAmmo> Snapshot { get; }
        public int RemainingAmmo => Snapshot.Count;

        public event Action<TAmmo> OnInserted;
        public event Action<TAmmo> OnRemoved;

        public TAmmo Peek(int offset = 0);
        public TAmmo Extract();
        public void Insert(TAmmo ammo);
    }

    public abstract class AmmoStorage<TAmmo> : IAmmoStorage<TAmmo> where TAmmo : IAmmoType
    {
        public IReadOnlyList<TAmmo> Snapshot => _ammo;

        private readonly List<TAmmo> _ammo;

        public event Action<TAmmo> OnInserted;
        public event Action<TAmmo> OnRemoved;

        public AmmoStorage() => _ammo = new List<TAmmo>();
        public AmmoStorage(IEnumerable<TAmmo> ammo) => _ammo = ammo.ToList();

        public TAmmo Peek(int offset = 0)
        {
            if (offset < 0 || offset >= _ammo.Count)
                return default;

            return _ammo[offset];
        }

        public abstract TAmmo Extract();
        public abstract void Insert(TAmmo ammo);

        protected TAmmo ExtractAt(int index)
        {
            var value = Peek(index);
            _ammo.RemoveAt(index);
            OnRemoved?.Invoke(value);

            return value;
        }

        protected void InsertAt(TAmmo ammo, int index)
        {
            if (index == _ammo.Count)
                _ammo.Add(ammo);
            else
                _ammo.Insert(index, ammo);

            OnInserted?.Invoke(ammo);
        }
    }

    public interface IAmmoLoader<TAmmo> where TAmmo : IAmmoType
    {
        public bool TryLoad(IAmmoSupply<TAmmo> supply, AmmoContainer<TAmmo> container, int count = -1);
    }

    public abstract class AmmoLoader<TAmmo> : IAmmoLoader<TAmmo> where TAmmo : IAmmoType
    {
        public bool TryLoad(IAmmoSupply<TAmmo> supply, AmmoContainer<TAmmo> container, int count = -1)
        {
            bool wasAmmoReplenished = false;

            if (count < 0)
            {
                while (supply.TryGet(x => container.CanAccept(x) && container.IsExpected(x) && container.CanBeInserted(x), out TAmmo cartridge))
                    wasAmmoReplenished = container.TryInsert(cartridge) || wasAmmoReplenished;

                return wasAmmoReplenished;
            }

            for (int i = 0; i < count; i++)
            {
                if(!supply.TryGet(x => container.CanAccept(x) && container.IsExpected(x) && container.CanBeInserted(x), out TAmmo cartridge))
                    return false;

                wasAmmoReplenished = container.TryInsert(cartridge) || wasAmmoReplenished;
            }

            return wasAmmoReplenished;
        }
    }

    public interface ILoadingPolicy<TAmmo> where TAmmo : IAmmoType
    {
        public IReadOnlyList<TAmmo> LoadingPattern { get; }

        public bool CanAccept(TAmmo ammo);
        public bool IsExpected(TAmmo candidate);
    }

    public class FreeLoadingPolicy<TAmmo> : ILoadingPolicy<TAmmo> where TAmmo : IAmmoType
    {
        public IReadOnlyList<TAmmo> LoadingPattern => null;

        public bool CanAccept(TAmmo ammo) => true;
        public bool IsExpected(TAmmo candidate) => true;
    }

    public class StaticLoadingPolicy<TAmmo> : ILoadingPolicy<TAmmo> where TAmmo : IAmmoType
    {
        public IReadOnlyList<TAmmo> LoadingPattern { get; }

        private FixedValueProvider<TAmmo> _requiredAmmo;

        public StaticLoadingPolicy(TAmmo ammo)
        {
            LoadingPattern = new TAmmo[] { ammo };

            _requiredAmmo = new FixedValueProvider<TAmmo>(ammo);
        }

        public bool CanAccept(TAmmo ammo) => ammo.Equals(_requiredAmmo.Value);
        public bool IsExpected(TAmmo candidate) => candidate.Equals(_requiredAmmo.Value);
    }

    public class SequentialLoadingPolicy<TAmmo> : ILoadingPolicy<TAmmo> where TAmmo : IAmmoType
    {
        public IReadOnlyList<TAmmo> LoadingPattern => _requiredAmmo.Sequence;

        private SequentialValueProvider<TAmmo> _requiredAmmo;

        public SequentialLoadingPolicy(IEnumerable<TAmmo> ammo)
        {
            _requiredAmmo = new SequentialValueProvider<TAmmo>(ammo);
        }

        public bool CanAccept(TAmmo ammo) => _requiredAmmo.Sequence.Contains(ammo);
        public bool IsExpected(TAmmo candidate) => _requiredAmmo.Peek(-1).Equals(candidate);
    }

    public interface ICapacityPolicy<TAmmo> where TAmmo : IAmmoType
    {
        public int MaxCapacity(IReadOnlyList<TAmmo> current);
        public int RemainingCapacity(IReadOnlyList<TAmmo> current);
        public bool CanBeInserted(IReadOnlyList<TAmmo> current, TAmmo candidate);
    }

    public class FixedCapacityPolicy<TAmmo> : ICapacityPolicy<TAmmo> where TAmmo : IAmmoType
    {
        private int _capacity;

        public FixedCapacityPolicy(int capacity) => _capacity = Math.Max(capacity, 0);

        public bool CanBeInserted(IReadOnlyList<TAmmo> current, TAmmo candidate) => current.Count < _capacity;
        public int MaxCapacity(IReadOnlyList<TAmmo> current) => _capacity;
        public int RemainingCapacity(IReadOnlyList<TAmmo> current) => _capacity - current.Count;
    }
}