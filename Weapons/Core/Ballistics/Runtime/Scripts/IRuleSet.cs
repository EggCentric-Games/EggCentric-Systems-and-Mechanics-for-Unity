namespace EggCentric.Ballistics
{
    public interface IRuleSet<in TContact, out TRequest> where TContact : IContact
    {
        public TRequest RetrieveFor(TContact contact);
    }
}