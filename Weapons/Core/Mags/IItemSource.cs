using System.Collections.Generic;

public interface IItemHandle<out TItem>
{
    public TItem Template { get; }
}

public interface IItemSource<TItem, out THandle> where THandle : IItemHandle<TItem>
{
    public IReadOnlyCollection<TItem> GetItemsRange();
    public THandle GetInstance(TItem template);
}

public interface IItemVault<in TItem, in THandle> where THandle : IItemHandle<TItem>
{
    public bool IsAcceptable(THandle instance);
    public bool Deposit(THandle instance);
}

public interface IItemStorage<TItem, THandle> : IItemSource<TItem, THandle>, IItemVault<TItem, THandle> where THandle : IItemHandle<TItem>
{

}

public class Test : IItemStorage<TestAmmoConfig, IItemHandle<TestAmmoConfig>>
{
    public bool Deposit(IItemHandle<TestAmmoConfig> instance)
    {
        throw new System.NotImplementedException();
    }

    public IItemHandle<TestAmmoConfig> GetInstance(TestAmmoConfig template)
    {
        throw new System.NotImplementedException();
    }

    public IReadOnlyCollection<TestAmmoConfig> GetItemsRange()
    {
        throw new System.NotImplementedException();
    }

    public bool IsAcceptable(IItemHandle<TestAmmoConfig> instance)
    {
        throw new System.NotImplementedException();
    }
}