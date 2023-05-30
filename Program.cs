namespace miniDI;
class Program
{

    public class AService { 
        public int Counter { get; set; } = 0;
    }
    public class BService { 
        public int Counter { get; set; } = 0;
    }

    public class CService { 
        public int Counter { get; set; } = 42;
    }

    public class BFactory : FactoryCreator<BService>{
        public BService Create() {
            BService b = new BService();
            return b;
        }
    }

    static void Main(string[] args)
    {
        DIHost host = new DIHost();

        host.Register<AService>(new AService(), DIHost.Lifetime.Singleton);
        host.Register<BService>(new BFactory(), DIHost.Lifetime.Transient);
        host.Register<CService>(null, DIHost.Lifetime.Transient);

        AService a = host.Resolve<AService>();
        Console.WriteLine(a.Counter);
        a.Counter++;
        Console.WriteLine(a.Counter);

        AService a2 = host.Resolve<AService>();
        Console.WriteLine(a2.Counter);

        BService b = host.Resolve<BService>();
        Console.WriteLine(b.Counter);
        b.Counter++;
        Console.WriteLine(b.Counter);

        BService b2 = host.Resolve<BService>();
        Console.WriteLine(b2.Counter);

        CService c = host.Resolve<CService>();
        Console.WriteLine(c.Counter);

        Console.WriteLine("Using Session");
        using(Session s = new Session(host)) {
            AService a3 = s.Resolve<AService>();
            Console.WriteLine(a3.Counter);
            a3.Counter++;
            Console.WriteLine(a3.Counter);

            AService a4 = s.Resolve<AService>();
            Console.WriteLine(a4.Counter);

            
            BService b3 = s.Resolve<BService>();
            Console.WriteLine(b3.Counter);
            b3.Counter++;
            Console.WriteLine(b3.Counter);

            Console.WriteLine("Here we expect the change against the sessionless version, the transient is recycled");
            BService b4 = s.Resolve<BService>();
            Console.WriteLine(b4.Counter);

            CService c2 = s.Resolve<CService>();
            Console.WriteLine(c2.Counter);
        }
    }
}
