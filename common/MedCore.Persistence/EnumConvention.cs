// <copyright file="EnumConvention.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace MedCore.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

/// <inheritdoc />
public class EnumConvention : IModelFinalizingConvention
{
    /// <inheritdoc/>
    public void ProcessModelFinalizing(IConventionModelBuilder modelBuilder, IConventionContext<IConventionModelBuilder> context)
    {
 #pragma warning disable EF1001
        var builder = new ModelBuilder((Model)modelBuilder.Metadata);
 #pragma warning restore EF1001
        Configure(builder);
    }

    private static bool IsValidEnum(IMutableProperty property)
    {
        var propertyType = property.ClrType;

        return propertyType.IsEnum;
    }

    private static void ApplyEnumLookupEntityType(ModelBuilder builder, IMutableProperty property)
    {
        var entityType = (IMutableEntityType)property.DeclaringType;
        var propertyType = property.ClrType;
        var concreteType = typeof(EnumTable<>).MakeGenericType(propertyType);

        if (builder.Model.FindEntityType(concreteType) != null)
        {
            return;
        }

        var enumLookupBuilder = builder.Entity(concreteType);
        var schema = GetOriginatingSchema(entityType);
        enumLookupBuilder.ToTable(propertyType.Name, schema);
        enumLookupBuilder.Property(nameof(EnumTable<>.Id)).ValueGeneratedNever();
        enumLookupBuilder.Property(nameof(EnumTable<>.Name)).HasMaxLength(200);
        enumLookupBuilder.HasKey(nameof(EnumTable<>.Id));

        var data = Enum.GetValues(propertyType)
            .Cast<object>()
            .Select(v => Activator.CreateInstance(concreteType, v))
            .Cast<object>()
            .ToArray();

        enumLookupBuilder.HasData(data);
    }

    private static string? GetOriginatingSchema(IMutableEntityType entityType)
    {
        var schema = entityType.GetAnnotations()
            .FirstOrDefault(a => a.Name == "Relational:Schema")
            ?.Value?.ToString();

        if (schema != null)
        {
            return schema;
        }

        var ownership = entityType.GetForeignKeys().FirstOrDefault(fk => fk.IsOwnership);
        return ownership != null
            ? GetOriginatingSchema(ownership.PrincipalEntityType)
            : null;
    }

    private void Configure(ModelBuilder builder)
    {
        var properties = builder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
            .Where(IsValidEnum)
            .ToArray();

        foreach (var property in properties)
        {
            ApplyEnumLookupEntityType(builder, property);

 #pragma warning disable EF1001
            var entityTypeBuilder = new EntityTypeBuilder((IMutableEntityType)property.DeclaringType);
 #pragma warning restore EF1001
            var concreteType = typeof(EnumTable<>).MakeGenericType(property.ClrType);

            entityTypeBuilder.HasOne(concreteType)
                .WithMany()
                .HasPrincipalKey(nameof(EnumTable<>.Id))
                .HasForeignKey(property.Name)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }

 #pragma warning disable SA1600
    private class EnumTable<T>
 #pragma warning restore SA1600
        where T : Enum
    {
 #pragma warning disable SA1600
        public EnumTable(T value)
 #pragma warning restore SA1600
        {
            Id = value;
            Name = value.ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumTable{T}"/> class.
        ///     For EF only.
        /// </summary>
        private EnumTable()
        {
        }

 #pragma warning disable SA1600
        public T Id { get; set; } = default!;
 #pragma warning restore SA1600

 #pragma warning disable SA1600
        public string Name { get; set; } = default!;
 #pragma warning restore SA1600
    }
}