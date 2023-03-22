using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FEnum;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

public class FEffectManager : FSingleton<FEffectManager>
{
    int instanceID;

    Dictionary<int, FProjectile> projectileMap = new Dictionary<int, FProjectile>();
    Dictionary<int, FEffect> effectMap = new Dictionary<int, FEffect>();

    private void Update()
    {
        foreach(var pair in projectileMap.Reverse())
        {
            pair.Value.Tick(Time.deltaTime);
        }

        foreach (var pair in effectMap.Reverse())
        {
            pair.Value.Tick(Time.deltaTime);
        }
    }

    public void AddProjectile(int InProjectileID, FObjectBase InOwner, Vector2 InStart, Vector2 InEnd)
    {
        FProjectile projectile = CreateProjectile(InProjectileID);
        projectile.Initialize(InProjectileID, InOwner, InEnd);
        projectile.WorldPosition = InStart;
    }

    public void AddProjectile(int InProjectileID, FObjectBase InOwner, Vector2 InStart, FObjectBase InTarget)
    {
        FProjectile projectile = CreateProjectile(InProjectileID);
        projectile.Initialize(InProjectileID, InOwner, InTarget);
        projectile.WorldPosition = InStart;
    }

    public void RemoveProjectile(int InInstanceID)
    {
        if(projectileMap.ContainsKey(InInstanceID))
        {
            GameObject.Destroy(projectileMap[InInstanceID].gameObject);
            projectileMap.Remove(InInstanceID);
        }
    }

    public void AddEffect(int InEffectID, FObjectBase InOwner, Vector2 InPosition)
    {
        FEffect effect = CreateEffect(InEffectID);
        effect.Initialize(InEffectID, InOwner);
        effect.WorldPosition = InPosition;
    }

    public void AddEffect(int InEffectID, FObjectBase InOwner, FObjectBase InTarget)
    {
        FEffect effect = CreateEffect(InEffectID);
        effect.Initialize(InEffectID, InOwner, InTarget);
        effect.WorldPosition = InTarget.WorldPosition;
    }

    public void RemoveEffect(int InInstanceID)
    {
        if (effectMap.ContainsKey(InInstanceID))
        {
            GameObject.Destroy(effectMap[InInstanceID].gameObject);
            effectMap.Remove(InInstanceID);
        }
    }

    private FProjectile CreateProjectile(int InProjectileID)
    {
        FProjectileData projectileData = FEffectDataManager.Instance.FindProjectileData(InProjectileID);
        if (projectileData == null)
            return null;

        GameObject prefab = Resources.Load<GameObject>(projectileData.prefab);
        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.SetParent(transform, true);

        FProjectile projectile = gameObject.AddComponent<FProjectile>();
        projectile.InstanceID = instanceID;

        projectileMap.Add(instanceID, projectile);

        ++instanceID;

        return projectile;
    }

    private FEffect CreateEffect(int InEffectID)
    {
        FEffectData effectData = FEffectDataManager.Instance.FindEffectData(InEffectID);
        if (effectData == null)
            return null;

        GameObject prefab = Resources.Load<GameObject>(effectData.prefab);
        if (prefab == null)
            return null;

        GameObject gameObject = Instantiate(prefab);
        gameObject.transform.SetParent(transform);

        FEffect effect = null;
        switch (effectData.type)
        {
            case SkillEffectType.Damage: effect = gameObject.AddComponent<FDamageEffect>(); break;
        }
        
        if(effect != null)
        {
            effect.InstanceID = instanceID;
            
            effectMap.Add(instanceID, effect);

            ++instanceID;
        }

        return effect;
    }
}
