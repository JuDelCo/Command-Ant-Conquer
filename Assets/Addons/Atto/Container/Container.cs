using System;
using System.Collections.Generic;
using Identifier = System.String;

namespace Atto
{
	public delegate void ContainerClassEvent(Type type, Identifier id);

	public partial class Container
	{
		public event ContainerClassEvent OnClassProvided;
		public event ContainerClassEvent OnFactoryProvided;
		public event ContainerClassEvent OnClassReplaced;
		public event ContainerClassEvent OnFactoryReplaced;

		private Dictionary<Type, Dictionary<Identifier, Func<object>>> classConstructors = null;
		private Dictionary<Type, Dictionary<Identifier, Func<object>>> classFactories = null;
		private Dictionary<Type, Dictionary<Identifier, object>> instanceContainer = null;

		public Container()
		{
			classConstructors = new Dictionary<Type, Dictionary<Identifier, Func<object>>>();
			classFactories = new Dictionary<Type, Dictionary<Identifier, Func<object>>>();
			instanceContainer = new Dictionary<Type, Dictionary<Identifier, object>>();
		}

		public T Get<T>(Identifier id)
		{
			return (T)(Get(typeof(T), id));
		}

		public void Provide<T>(Identifier id, Func<T> classConstructor)
		{
			Provide(typeof(T), id, () =>
			{
				return classConstructor();
			});
		}

		public void ProvideFactory<T>(Identifier id, Func<T> classConstructor)
		{
			ProvideFactory(typeof(T), id, () =>
			{
				return classConstructor();
			});
		}

		private object Get(Type type, Identifier id)
		{
			object instance = null;

			if(classFactories.ContainsKey(type) && classFactories[type].ContainsKey(id))
			{
				try
				{
					instance = classFactories[type][id]();
				}
				catch (Exception)
				{
					throw new Exception(string.Format("Error creating an instance of type '{0}' with id '{1}' using a factory method", type.ToString(), id));
				}
			}
			else
			{
				if(classConstructors.ContainsKey(type) && classConstructors[type].ContainsKey(id))
				{
					if(instanceContainer.ContainsKey(type) && instanceContainer[type].ContainsKey(id))
					{
						instance = instanceContainer[type][id];
					}
					else
					{
						if(!instanceContainer.ContainsKey(type))
						{
							instanceContainer.Add(type, new Dictionary<Identifier, object>());
						}

						try
						{
							instance = classConstructors[type][id]();
						}
						catch (Exception)
						{
							throw new Exception(string.Format("Error creating an instance of type '{0}' with id '{1}'", type.ToString(), id));
						}

						instanceContainer[type].Add(id, instance);
					}
				}
			}

			if(instance == null)
			{
				throw new NullReferenceException(string.Format("No class of type '{0}' with id '{1}' found", type.ToString(), id));
			}

			return instance;
		}

		private void Provide(Type type, Identifier id, Func<object> classConstructor)
		{
			if(CheckDuplicatedClass(type, id))
			{
				if (OnClassReplaced != null)
				{
					OnClassReplaced(type, id);
				}
			}

			SetClassConstructor(type, id, classConstructor);

			if (OnClassProvided != null)
			{
				OnClassProvided(type, id);
			}
		}

		private void ProvideFactory(Type type, Identifier id, Func<object> classConstructor)
		{
			if(CheckDuplicatedClass(type, id))
			{
				if (OnFactoryReplaced != null)
				{
					OnFactoryReplaced(type, id);
				}
			}

			SetClassFactory(type, id, classConstructor);

			if (OnFactoryProvided != null)
			{
				OnFactoryProvided(type, id);
			}
		}

		private bool CheckDuplicatedClass(Type type, Identifier id)
		{
			bool duplicated = false;

			if(classConstructors.ContainsKey(type))
			{
				duplicated |= classConstructors[type].ContainsKey(id);
			}

			if(classFactories.ContainsKey(type))
			{
				duplicated |= classFactories[type].ContainsKey(id);
			}

			return duplicated;
		}

		private void SetClassConstructor(Type type, Identifier id, Func<object> classConstructor)
		{
			RemoveClassFactory(type, id);

			if(classConstructors.ContainsKey(type))
			{
				RemoveClassConstructor(type, id);
			}
			else
			{
				classConstructors.Add(type, new Dictionary<Identifier, Func<object>>());
			}

			classConstructors[type].Add(id, classConstructor);
		}

		private void RemoveClassConstructor(Type type, Identifier id)
		{
			if(classConstructors.ContainsKey(type))
			{
				classConstructors[type].Remove(id);

				if(instanceContainer.ContainsKey(type))
				{
					instanceContainer[type].Remove(id);
				}
			}
		}

		private void SetClassFactory(Type type, Identifier id, Func<object> classConstructor)
		{
			RemoveClassConstructor(type, id);

			if(classFactories.ContainsKey(type))
			{
				RemoveClassFactory(type, id);
			}
			else
			{
				classFactories.Add(type, new Dictionary<Identifier, Func<object>>());
			}

			classFactories[type].Add(id, classConstructor);
		}

		private void RemoveClassFactory(Type type, Identifier id)
		{
			if(classFactories.ContainsKey(type))
			{
				classFactories[type].Remove(id);
			}
		}
	}
}
