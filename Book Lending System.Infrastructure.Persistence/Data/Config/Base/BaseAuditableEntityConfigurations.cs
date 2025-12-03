using Book_Lending_System.Core.Entities.Base;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Book_Lending_System.Infrastructure.Persistence.Data.Config.Base
{
    public class BaseAuditableEntityConfigurations<TEntity, TKey>: BaseEntityConfigurations<TEntity, TKey>
        where TEntity : BaseAuditableEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.CreatedOn)
                 .IsRequired()
                 ;

            builder.Property(e => e.CreatedBy)
                     .IsRequired();

            builder.Property(e => e.LastModifiedBy)
                     .IsRequired()
                     ;

            builder.Property(e => e.LastModifiedOn)
                .IsRequired();
        }
    }
}
