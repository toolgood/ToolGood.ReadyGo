using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using ToolGood.ReadyGo3.PetaPoco.Core;
using ToolGood.ReadyGo3.PetaPoco.Internal;

namespace ToolGood.ReadyGo3.PetaPoco
{
    /// <summary>
    /// 
    /// </summary>
    public static class Mappers
    {
        private static StandardMapper _mapper = new StandardMapper();

        /// <summary>
        ///     Retrieve the IMapper implementation to be used for a specified POCO type.
        /// </summary>
        /// <param name="entityType">The entity type to get the mapper for.</param>
        /// <returns>The mapper for the given type.</returns>
        public static StandardMapper GetMapper(Type entityType)
        {
            return _mapper;
   
        }

        public static StandardMapper GetMapper()
        {
            return _mapper;

        }

    }
}