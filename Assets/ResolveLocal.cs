using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;


//[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
//public class ResolveDependencies : Attribute
//{
//    public ResolveDependencies()
//    {

//    }
//}
namespace ResolutionFramework
{


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ResolveLocal : ResolveAttribute
    {

    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ResolveByTag : ResolveAttribute
    {
        public string Tag;
        public ResolveByTag(string tag)
        {
            Tag = tag;
        }

    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class Resolve : ResolveAttribute
    {
        public string Kernel = "Kernel";
        public Resolve()
        {

        }
        public Resolve(string kernel)
        {
            Kernel = kernel;
        }

    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ResolveByName : ResolveAttribute
    {
        public string Name;
        public ResolveByName(string name)
        {
            Name = name;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ResolveInChildren : ResolveAttribute
    {
        public static Component Resolve(GameObject gameObject, Type t)
        {
            return gameObject.GetComponentInChildren(t);
        }
    }

    public abstract class ResolveAttribute : Attribute
    {

    }

    public static class MonoBehaviourExtensions
    {
        public static void Resolve(this MonoBehaviour ob)
        {
            IResolver resolver = new Resolver();
            var fields = ob.GetType().GetFields().Where(field => Attribute.IsDefined(field, typeof(ResolveAttribute)));
            var local = fields.Where(field => Attribute.IsDefined(field, typeof(ResolveLocal)));
            local.ToList().ForEach(p =>
                p.SetValue(ob, resolver.ResolveLocal(ob.gameObject, p.FieldType))
            );
            var child = fields.Where(field => Attribute.IsDefined(field, typeof(ResolveInChildren)));
            child.ToList().ForEach(p =>
                p.SetValue(ob, resolver.ResolveInChildren(ob.gameObject, p.FieldType))
            );
            var byTag = fields.Where(field => Attribute.IsDefined(field, typeof(ResolveByTag)));
            byTag.ToList().ForEach(p =>
                p.SetValue(ob, resolver.ResolveByTag(
                    (Attribute.GetCustomAttribute(p, typeof(ResolveByTag)) as ResolveByTag).Tag,
                    p.FieldType))
            );
            var byName = fields.Where(field => Attribute.IsDefined(field, typeof(ResolveByName)));
            byName.ToList().ForEach(p =>
                p.SetValue(ob, resolver.ResolveByName(
                    (Attribute.GetCustomAttribute(p, typeof(ResolveByName)) as ResolveByName).Name,
                    p.FieldType))
            );

            var byKernel = fields.Where(field => Attribute.IsDefined(field, typeof(Resolve)));
            byKernel.ToList().ForEach(p =>
                p.SetValue(ob, resolver.Resolve(p.FieldType,
                    (Attribute.GetCustomAttribute(p, typeof(Resolve)) as Resolve).Kernel
                    ))
            );
        }
    }

    public class Resolver : IResolver
    {
        private Kernel Kernel;
        public static readonly string DefaultKernel = "Kernel";

        public Resolver(string kernelName)
        {
            Kernel = FindKernel(kernelName);
        }

        public Resolver(Kernel kernel)
        {
            Kernel = kernel;
        }

        public Resolver()
        {
            Kernel = FindKernel(DefaultKernel);
        }
        private Kernel FindKernel(string name)
        {
            var kernelObject = GameObject.FindWithTag(name);
            return kernelObject.GetComponent<Kernel>();
        }

        public Component ResolveByName(string name, Type t)
        {
            var find = GameObject.Find(name);
            if (find == null) return null;

            return find.GetComponent(t);
        }
        public Component ResolveByTag(string name, Type t)
        {
            var taggedFind = GameObject.FindGameObjectWithTag(name);
            if (taggedFind == null) return null;

            return taggedFind.GetComponent(t);
        }

        public Component ResolveInChildren(GameObject gameObject, Type t)
        {
            //prevents the attribute from returning the parent
            return gameObject.GetComponentsInChildren(t,true).FirstOrDefault(g => g.gameObject != gameObject);
        }

        public Component ResolveLocal(GameObject gameObject, Type t)
        {
            return gameObject.GetComponent(t);
        }

        public Component Resolve(Type t,string kernelName = "Kernel")
        {
           if (Kernel == null || Kernel.Modules == null || !Kernel.Modules.Any()) return null;
           return Kernel.Modules.FirstOrDefault(m => m.GetType() == t);
        }

    }

    

    //Attribute.GetCustomAttribute(t, typeof (DeveloperAttribute));

    public interface IResolver
    {
        Component ResolveLocal(GameObject gameObject, Type t);
        Component ResolveByName(string name, Type t);
        Component ResolveByTag(string tag, Type t);
        Component Resolve(Type t,string kernelName);
        Component ResolveInChildren(GameObject gameObject, Type t);
    }

}