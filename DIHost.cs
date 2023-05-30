namespace miniDI; 

using System.Reflection;

public interface FactoryCreator<T> {
    T Create();
}

public class DIHost {
    private static bool HasDefaultConstructor(Type type)
    {
        ConstructorInfo defaultConstructor = type.GetConstructor(Type.EmptyTypes);
        return defaultConstructor != null;
    }

    public enum Lifetime {
        Transient,
        Singleton
    }

    public bool Register<T>(object instance, Lifetime lifetime) where T : class {
        switch(lifetime) {
            case Lifetime.Transient:
                if(instance == null) {
                    if(!HasDefaultConstructor(typeof(T))) {
                        return false;
                    }
                     _TransientFactories[typeof(T)] = null;
                     return true; 
                }
                //validate that instance is a FactoryCreator<T> to avoid runtime errors
                if(!(instance is FactoryCreator<T>)) {
                    return false;
                }
                _TransientFactories[typeof(T)] = (FactoryCreator<T>)instance;
                return true;
            case Lifetime.Singleton:
                if(!_SingletonServices.ContainsKey(typeof(T))) {
                    _SingletonServices[typeof(T)] = (T)instance;
                    return true;
                }
                return false;
            default:
                return false;
        }
    }

    public T? Resolve<T>() where T : class {
        if(_SingletonServices.ContainsKey(typeof(T))) {
            return (T)_SingletonServices[typeof(T)];
        }
        if(_TransientFactories.ContainsKey(typeof(T))) {
            object factory = _TransientFactories[typeof(T)]; 
            if (factory == null) {
                return (T)Activator.CreateInstance(typeof(T));
            }
            else 
                return (T) ((FactoryCreator<T>)factory).Create();
        }
        return null;
    }

     public Session NewSession()
    {
        return new Session(this);
    }

    private Dictionary<Type, object> _SingletonServices = new Dictionary<Type, object>();
    private Dictionary<Type, object> _TransientFactories = new Dictionary<Type, object>();

}

public class Session : IDisposable {
    public Session(DIHost host) {
        _host = host;
    }

    public T? Resolve<T>() where T : class {

        if(_resolvedObjects.ContainsKey(typeof(T))) {
            return (T)_resolvedObjects[typeof(T)];
        }
        else
        {
            T? resolved = _host.Resolve<T>();
            if(resolved != null) {
                _resolvedObjects[typeof(T)] = resolved;
            }
            return resolved;
        }
    }

    public void Dispose()
    {
        _resolvedObjects.Clear();
    }

    private readonly DIHost _host;
     private readonly Dictionary<Type, object> _resolvedObjects = new Dictionary<Type, object>();

}