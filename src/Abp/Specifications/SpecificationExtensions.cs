using JetBrains.Annotations;
using System;

namespace Abp.Specifications
{
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that both the current and
        /// the given specification must be satisfied by the given object.
        /// </summary>
        /// <param name="specification">The specification</param>
        /// <param name="andSpecification">The specification instance with which the current specification is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public static Specification<T> And<T>([NotNull] this Specification<T> specification, [NotNull] Specification<T> andSpecification)
            where T : class
        {
            Check.NotNull(specification, nameof(specification));
            Check.NotNull(andSpecification, nameof(andSpecification));

            return new AndSpecification<T>(specification, andSpecification);
        }

        /// <summary>
        /// Combines the current specification instance with another specification instance
        /// and returns the combined specification which represents that either the current or
        /// the given specification should be satisfied by the given object.
        /// </summary>
        /// <param name="specification">The specification</param>
        /// <param name="orSpecification">The specification instance with which the current specification
        /// is combined.</param>
        /// <returns>The combined specification instance.</returns>
        public static Specification<T> Or<T>([NotNull] this Specification<T> specification, [NotNull] Specification<T> orSpecification)
            where T : class
        {
            Check.NotNull(specification, nameof(specification));
            Check.NotNull(orSpecification, nameof(orSpecification));

            return new OrSpecification<T>(specification, orSpecification);
        }

        /// <summary>
        /// Reverses the current specification instance and returns a specification which represents
        /// the semantics opposite to the current specification.
        /// </summary>
        /// <returns>The reversed specification instance.</returns>
        public static Specification<T> Not<T>([NotNull] this Specification<T> specification)
            where T : class
        {
            Check.NotNull(specification, nameof(specification));

            return new NotSpecification<T>(specification);
        }

        public static Specification<T> AndIf<T>([NotNull] this Specification<T> specification, bool condition, [NotNull] Specification<T> andSpecification)
            where T : class
        {
            Check.NotNull(specification, nameof(specification));
            Check.NotNull(andSpecification, nameof(andSpecification));

            if (condition)
            {
                return new AndSpecification<T>(specification, andSpecification);
            }
            else
            {
                return specification;
            }
        }

        public static Specification<T> OrIf<T>([NotNull] this Specification<T> specification, bool condition, [NotNull] Specification<T> orSpecification)
            where T : class
        {
            Check.NotNull(specification, nameof(specification));
            Check.NotNull(orSpecification, nameof(orSpecification));

            if (condition)
            {
                return new OrSpecification<T>(specification, orSpecification);
            }
            else
            {
                return specification;
            }
        }

        public static Specification<T> AndIf<T>([NotNull] this Specification<T> specification, bool condition, [NotNull] Func<Specification<T>> andSpecificationFunc)
            where T : class
        {
            Check.NotNull(specification, nameof(specification));
            Check.NotNull(andSpecificationFunc, nameof(andSpecificationFunc));

            if (condition)
            {
                return new AndSpecification<T>(specification, andSpecificationFunc());
            }
            else
            {
                return specification;
            }
        }

        public static Specification<T> OrIf<T>([NotNull] this Specification<T> specification, bool condition, [NotNull] Func<Specification<T>> orSpecificationFunc)
            where T : class
        {
            Check.NotNull(specification, nameof(specification));
            Check.NotNull(orSpecificationFunc, nameof(orSpecificationFunc));

            if (condition)
            {
                return new OrSpecification<T>(specification, orSpecificationFunc());
            }
            else
            {
                return specification;
            }
        }
    }
}