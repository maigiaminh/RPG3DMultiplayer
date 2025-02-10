using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public enum WeaponType
{
    Unarmed = 0,
    GreatSword = 1,
    Bow = 2,
    Shield_and_Mace = 3,
    DualKnife = 4,
    WizardStaff = 5,
    SpellBook = 6
}


public class WeaponController : MonoBehaviour
{
    [Header("Current Equiped")]
    public Weapon weaponInLeftHand;
    public Weapon weaponInRightHand;
    public WeaponDamage weaponDamageLeftHand;
    public WeaponDamage weaponDamageRightHand;
    public int primaryWeapon;
    public Weapon weaponLogic;
    public GameObject weaponSealthParent;
    public GameObject rightWeaponsParent;
    public GameObject leftWeaponsParent;
    public ProjectilePool projectilePool;
    public Transform shootPoint;
    public ParticleSystem Effect;
    public string weaponName;
    private List<WeaponModel> _weaponModels = new List<WeaponModel>();
    private List<Weapon> rightWeapons = new List<Weapon>();
    private List<Weapon> leftWeapons = new List<Weapon>();

    private void Start() {
        InitConfigWeaponModels();
    }

    public void ToogleRightWeapon(bool toggle)
    {
        GameObject weapon = weaponInRightHand.transform.GetChild(0).gameObject;
        weapon.SetActive(toggle);
    }

    public void ToogleLeftWeapon(bool toggle)
    {
        GameObject weapon = weaponInLeftHand.transform.GetChild(0).gameObject;
        weapon.SetActive(toggle);
    }

    public void EnableRight()
    {
        ToogleRightWeapon(true);
    }

    public void DisableRight()
    {
        ToogleRightWeapon(false);
    }

    public void EnableLeft()
    {
        ToogleLeftWeapon(true);
    }

    public void DisableLeft()
    {
        ToogleLeftWeapon(false);
    }


    public void ChangeWeapon(int type, string name)
    {
        weaponInLeftHand = null;
        weaponInRightHand = null;
        
        Debug.Log("TYPE: " + type);
        Debug.Log("TYPE RIGHT WEAPON: " + rightWeapons.Count());
        foreach(Weapon weapon in rightWeapons) {
            if((int)weapon.weaponType != type) continue;

            if(type == 2){
                if(weapon.Name != "Arrow") continue;
            }
            else if(type == 3){
                if(weapon.Name != "SM_Wep_Sceptre_06") continue;
            }
            else{
                if(weapon.Name != name) continue;
            }
            
            weaponInRightHand = weapon;
            foreach(Transform child in weaponInRightHand.transform) {
                if (!child.TryGetComponent<WeaponDamage>(out weaponDamageRightHand)) continue;
            }
        }

        foreach(Weapon weapon in leftWeapons){
            if((int)weapon.weaponType != type) continue;
            if(weapon.Name != name) continue;

            weaponInLeftHand = weapon;
            foreach(Transform child in weaponInLeftHand.transform) {
                if (!child.TryGetComponent<WeaponDamage>(out weaponDamageLeftHand)) continue;
            }
        }

        weaponLogic = weaponInRightHand;
        if(weaponLogic.projectileSpawnPoint != null) shootPoint = weaponLogic.projectileSpawnPoint;
    }

    public void EquipWeaponModel(int type, string weaponName)
    {
        ToggleWeapon("models", type, true, weaponName);
        ToggleWeapon("left", type, false);
        ToggleWeapon("right", type, false);

        primaryWeapon = type;
        this.weaponName = weaponName;
    }


    public void UnEquipWeaponModel(int type, string weaponName)
    {
        ToggleWeapon("models", type, false, weaponName);
        ToggleWeapon("left", type, false, weaponName);
        ToggleWeapon("right", type, false, weaponName);

        primaryWeapon = 0;
        ChangeWeapon(0, "Unarmed");
        GameEventManager.Instance.PlayerEvents.UnEquipWeapon();
    }

    public void SheathWeapon(int type){
        ToggleWeapon("models", type, true, weaponName);
        ToggleWeapon("left", type, false, weaponName);
        ToggleWeapon("right", type, false, weaponName);  
    }

    public void UnsheathWeapon(int type){
        ToggleWeapon("models", type, false, weaponName);
        ToggleWeapon("left", type, true, weaponName);
        ToggleWeapon("right", type, true, weaponName);   
        
        if(type == 2){
            foreach(var model in _weaponModels)
            {
                if(model.name != "Sheath-Arrow-Quiver") continue;
                model.gameObject.SetActive(true);
            }
        }
    }
    
    private void ToggleWeapon(string parent, int type, bool enable, string name = null){
        switch(parent){
            case "left":
                foreach(var left in leftWeapons){
                    if(type == 0 && left.Name == "Unarmed"){
                        left.gameObject.SetActive(enable);
                    }
                    else{
                        if((int)left.weaponType != type) continue;
                        if(left.Name != name) continue;

                        left.gameObject.SetActive(enable);
                    }
                }

                break;
                
            case "right":
                foreach(var right in rightWeapons){
                    if(type == 0 && right.Name == "Unarmed"){
                        right.gameObject.SetActive(enable);
                    }
                    else if(type == 2 && right.name == "Arrow"){
                        right.gameObject.SetActive(enable);
                    }
                    else if(type == 3 && right.name == "SM_Wep_Sceptre_06"){
                        right.gameObject.SetActive(enable);
                    }
                    else{
                        if((int)right.weaponType != type) continue;
                        if(right.Name != name) continue;

                        right.gameObject.SetActive(enable);
                    }
                    
                }

                break;

            case "models":
            
                foreach(var model in _weaponModels)
                {
                    if(type == 2 && model.name == "Sheath-Arrow-Quiver"){
                        model.gameObject.SetActive(enable);
                    }
                    else if(type == 3 && model.name == "SM_Wep_Sceptre_06"){
                        model.gameObject.SetActive(enable);
                    }

                    if((int)model.weaponType != type) continue;
                    if(model.Name != name) continue;

                    model.gameObject.SetActive(enable);
                }

                break;
            
            default:
                break;
        }
    }

    private void InitConfigWeaponModels(){
        foreach(Transform child in weaponSealthParent.transform){
            if(child.childCount > 0){
                foreach(Transform c in child)
                {  
                    if(c.TryGetComponent<WeaponModel>(out WeaponModel weaponModel)){
                        weaponModel.gameObject.SetActive(false);
                        _weaponModels.Add(weaponModel);
                        Debug.Log("WeaponModel added: " + weaponModel.Name);    
                    }
                }
            }
        }
                
        Weapon[] rWeapons = rightWeaponsParent.GetComponentsInChildren<Weapon>();
        Weapon[] lWeapons = leftWeaponsParent.GetComponentsInChildren<Weapon>();

        foreach (Weapon weapon in rWeapons){
            weapon.gameObject.SetActive(false);

            rightWeapons.Add(weapon);
            Debug.Log("Right Weapon added: " + weapon.Name);
        }

        foreach (Weapon weapon in lWeapons){
            weapon.gameObject.SetActive(false);

            leftWeapons.Add(weapon);
            Debug.Log("Left Weapon added: " + weapon.Name);
        }
        
        /*
        foreach(Transform child in rightWeaponsParent.transform){
            child.gameObject.SetActive(false);
            Weapon weapon;

            if (!child.TryGetComponent<Weapon>(out weapon)) continue;
            
            rightWeapons.Add(weapon);
            Debug.Log("Right Weapon added: " + weapon.Name);

        }
        
        foreach(Transform child in leftWeaponsParent.transform){
            child.gameObject.SetActive(false);
            Weapon weapon;

            if (!child.TryGetComponent<Weapon>(out weapon)) continue;
            leftWeapons.Add(weapon);
            Debug.Log("Left Weapon added: " + weapon.Name);
        }
        */
        ChangeWeapon(0, "Unarmed");
        ToggleWeapon("left", 0, true, "Unarmed");
        ToggleWeapon("right", 0, true, "Unarmed");   
    }

    public void Shoot(int type){
        PlayerStateMachine stateMachine;

        if(TryGetComponent<PlayerStateMachine>(out stateMachine)){
            if(type == 2){
                Arrow arrow = projectilePool.GetArrow();
                arrow.transform.position = shootPoint.position;
                arrow.transform.rotation = stateMachine.MainCameraTransform.rotation;
                Rigidbody rb = arrow.GetComponent<Rigidbody>();
                rb.linearVelocity = stateMachine.MainCameraTransform.forward * 20;
                ToggleWeapon("right", type, false);
            }
            else if(type == 5){
                WizardProjectile projectile = projectilePool.GetProjectile();
                projectile.transform.position = shootPoint.position;
                projectile.transform.rotation = stateMachine.MainCameraTransform.rotation;

                if(stateMachine.Targeter.CurrentTarget != null){
                    projectile.UpdateTarget(stateMachine.Targeter.CurrentTarget.transform, Vector3.up);
                    projectile.useForwardMode = false;
                }
                else{
                    projectile.forwardDirection = stateMachine.MainCameraTransform.forward.normalized;
                    projectile.useForwardMode = true;
                }
                
                Effect.Play();
            }
        }
        
    }

    public void EnableProjectile(int type){
        ToggleWeapon("right", type, true);
    }

    public bool CanEquip(int weaponType){
        var skillManager = GetComponent<SkillManager>();
        var characterTypeInt = skillManager.GetCharacterTypeInt();

        if(weaponType == characterTypeInt) return true;
        
        return false;
    }
}

