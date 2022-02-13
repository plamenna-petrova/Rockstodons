namespace Rockstodons.Data.Common.Repositories
{
    using System.Linq;

    using Rockstodons.Data.Common.Models;

    public interface IDeletableEntityRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : class, IDeletableEntity
    {
        IQueryable<TEntity> AllWithDeleted();

        IQueryable<TEntity> AllAsNoTrackingWithDeleted();

        void HardDelete(TEntity entity);

        void Undelete(TEntity entity);
    }
}
