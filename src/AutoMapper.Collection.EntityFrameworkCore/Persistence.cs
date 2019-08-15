using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AutoMapper.EntityFrameworkCore
{
    public class Persistence<TTo> : IPersistence<TTo>
        where TTo : class
    {
        private readonly DbSet<TTo> _sourceSet;
        private readonly IMapper _mapper;

        public Persistence(DbSet<TTo> sourceSet, IMapper mapper)
        {
            _sourceSet = sourceSet;
            _mapper = mapper;
        }

        public TTo InsertOrUpdate<TFrom>(TFrom from)
            where TFrom : class
        {
            return InsertOrUpdate(typeof(TFrom), from);
        }

        public Task<TTo> InsertOrUpdateAsync<TFrom>(TFrom from, CancellationToken cancellationToken = default(CancellationToken))
            where TFrom : class
        {
            return InsertOrUpdateAsync(typeof(TFrom), from, cancellationToken);
        }

        public TTo InsertOrUpdate(Type type, object from)
        {
            var equivExpr = GetEquivalenceExpression(type, from);
            if (equivExpr == null)
                throw new ArgumentException($"Could not retreive equivalency expression for mapping {type.Name} --> {typeof(TTo).Name}");

            var to = _sourceSet.FirstOrDefault(equivExpr);

            return MapObject(type, from, to);
        }

        public async Task<TTo> InsertOrUpdateAsync(Type type, object from, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var equivExpr = GetEquivalenceExpression(type, from);
            if (equivExpr == null)
                throw new ArgumentException($"Could not retreive equivalency expression for mapping {type.Name} --> {typeof(TTo).Name}");

            var to = await _sourceSet.FirstOrDefaultAsync(equivExpr, cancellationToken).ConfigureAwait(false);

            return MapObject(type, from, to);
        }

        public void Remove<TFrom>(TFrom from)
            where TFrom : class
        {
            var equivExpr = GetEquivalenceExpression(from);
            if (equivExpr == null)
                return;
            var to = _sourceSet.FirstOrDefault(equivExpr);

            if (to != null)
                _sourceSet.Remove(to);
        }

        public async Task RemoveAsync<TFrom>(TFrom from, CancellationToken cancellationToken = default(CancellationToken))
            where TFrom : class
        {
            cancellationToken.ThrowIfCancellationRequested();

            var equivExpr = GetEquivalenceExpression(from);
            if (equivExpr == null)
                return;
            var to = await _sourceSet.FirstOrDefaultAsync(equivExpr, cancellationToken).ConfigureAwait(false);

            if (to != null)
                _sourceSet.Remove(to);
        }

        private TTo MapObject(Type type, object from, TTo to)
        {
            if (to == null)
            {
                to = (TTo)(_mapper?.Map(from, type, typeof(TTo)) ?? Mapper.Map(from, type, typeof(TTo)));
                _sourceSet.Add(to);
            }
            else
            {
                if (_mapper == null)
                    Mapper.Map(from, to);
                else
                    _mapper.Map(from, to);
            }
            return to;
        }

        private Expression<Func<TTo, bool>> GetEquivalenceExpression<TFrom>(TFrom from)
        {
            return GetEquivalenceExpression(typeof(TFrom), from);
        }

        private Expression<Func<TTo, bool>> GetEquivalenceExpression(Type type, object from)
        {
            return _mapper == null
                ? Mapper.Map(from, type, typeof(Expression<Func<TTo, bool>>)) as Expression<Func<TTo, bool>>
                : _mapper.Map(from, type, typeof(Expression<Func<TTo, bool>>)) as Expression<Func<TTo, bool>>;
        }
    }
}