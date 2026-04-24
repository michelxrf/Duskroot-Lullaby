using UnityEngine;

public class Ranged : WeaponBehavior
{
    public override void Execute()
    {
        base.Execute();
    }

    public override void ImpactFrame()
    {
        base.ImpactFrame();

        var projectile = RunnerBootstrap.Instance.Runner.Spawn(weaponData.projectilePrefab, defaultTarget.position, transform.rotation);
        projectile.GetBehaviour<Projectile>().SetUp(new Vector3(transform.forward.x, 0, transform.forward.z), weaponData, gameObject);
    }
}
