using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FastDynamicMemberAccessor
{
    /// <summary>
    /// The PropertyAccessor class provides fast dynamic access
    /// to a property of a specified target class.
    /// </summary>
    internal class FieldAccessor : MemberAccessor
    {
        private readonly Type _propertyType;
        private readonly bool _canRead;
        private readonly bool _canWrite;

        /// <summary>Creates a new property accessor.</summary>
        internal FieldAccessor(FieldInfo fieldInfo)
            : base(fieldInfo)
        {
            _canRead = true;
            _canWrite = !fieldInfo.IsLiteral && !fieldInfo.IsInitOnly;
            _propertyType = fieldInfo.FieldType;
        }

        /// <summary>The Type of the Property being accessed.</summary>
        internal override Type MemberType => _propertyType;

        /// <summary>Whether or not the Property supports read access.</summary>
        internal override bool CanRead => _canRead;

        /// <summary>Whether or not the Property supports write access.</summary>
        internal override bool CanWrite => _canWrite;

        protected override void _EmitSetter(TypeBuilder myType)
        {
            var parameterTypes = new Type[2]
            {
                typeof(object),
                typeof(object)
            };
            Type returnType = null;
            var ilGenerator = myType
                .DefineMethod("Set", MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes)
                .GetILGenerator();
            var field = _targetType.GetField(_fieldName);
            if (field != null)
            {
                var fieldType = field.FieldType;
                ilGenerator.DeclareLocal(fieldType);
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Castclass, _targetType);
                ilGenerator.Emit(OpCodes.Ldarg_2);
                if (fieldType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Unbox, fieldType);
                    if (s_TypeHash[fieldType] != null)
                    {
                        var opcode = (OpCode)s_TypeHash[fieldType];
                        ilGenerator.Emit(opcode);
                    }
                    else
                        ilGenerator.Emit(OpCodes.Ldobj, fieldType);
                }
                else
                    ilGenerator.Emit(OpCodes.Castclass, fieldType);

                ilGenerator.Emit(OpCodes.Stfld, field);
            }
            else
                ilGenerator.ThrowException(typeof(MissingMethodException));

            ilGenerator.Emit(OpCodes.Ret);
        }

        protected override void _EmitGetter(TypeBuilder myType)
        {
            var parameterTypes = new Type[1]
            {
                typeof(object)
            };
            var returnType = typeof(object);
            var ilGenerator = myType
                .DefineMethod("Get", MethodAttributes.Public | MethodAttributes.Virtual, returnType, parameterTypes)
                .GetILGenerator();
            var field = _targetType.GetField(_fieldName);
            if (field != null)
            {
                ilGenerator.DeclareLocal(typeof(object));
                ilGenerator.Emit(OpCodes.Ldarg_1);
                ilGenerator.Emit(OpCodes.Castclass, _targetType);
                ilGenerator.Emit(OpCodes.Ldfld, field);
                if (field.FieldType.IsValueType)
                    ilGenerator.Emit(OpCodes.Box, field.FieldType);
                ilGenerator.Emit(OpCodes.Stloc_0);
                ilGenerator.Emit(OpCodes.Ldloc_0);
            }
            else
                ilGenerator.ThrowException(typeof(MissingMethodException));

            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}