using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class CharacterMultiplaySkins : NetworkBehaviour
{
    [HideInInspector] public NetworkVariable<FixedString32Bytes> CurrentSkin = new NetworkVariable<FixedString32Bytes>();
    [HideInInspector] public NetworkVariable<FixedString32Bytes> CurrentHair = new NetworkVariable<FixedString32Bytes>();
    [HideInInspector] public NetworkVariable<FixedString32Bytes> CurrentBeard = new NetworkVariable<FixedString32Bytes>();
    [HideInInspector] public NetworkVariable<FixedString32Bytes> CurrentHelmet = new NetworkVariable<FixedString32Bytes>();


    public Transform SkinContentParent;
    public Transform HairContentParent;
    public Transform BeardContentParent;
    public Transform HelmetContentParent;


    public const string InitialSkin = "SM_Chr_Soldier_Female_02";
    public const string InitialHair = "SM_Chr_Hair_Female_01";
    public const string InitialBeard = "SM_Chr_Hair_Beard_01";
    public const string InitialHelmet = "SM_Chr_Attach_Crown_Leaf_01";

    private void Awake()
    {
        CurrentSkin.OnValueChanged += (previous, current) => ChangeSkin(current.ToString());
        CurrentHair.OnValueChanged += (previous, current) => ChangeHair(current.ToString());
        CurrentBeard.OnValueChanged += (previous, current) => ChangeBeard(current.ToString());
        CurrentHelmet.OnValueChanged += (previous, current) => ChangeHelmet(current.ToString());
    }
    private void Start()
    {
        if (!IsOwner) return;

        var container = OutfitContainer.Instance;

        var hair = container != null ? container.OutfitData.HairName : InitialHair;
        var beard = container != null ? container.OutfitData.BeardName : InitialBeard;
        var helmet = container != null ? container.OutfitData.HelmetName : InitialHelmet;
        var skin = container != null ? container.OutfitData.SkinName : InitialSkin;


        UpdateOutfitsServerRpc(hair, beard, helmet, skin);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Cập nhật giao diện khi client được spawn
        ChangeSkin(CurrentSkin.Value.ToString());
        ChangeHair(CurrentHair.Value.ToString());
        ChangeBeard(CurrentBeard.Value.ToString());
        ChangeHelmet(CurrentHelmet.Value.ToString());
    }



    [ServerRpc]
    private void UpdateOutfitsServerRpc(string hair, string beard, string helmet, string skin)
    {
        // CurrentHair.Value = hair + "_temp";
        CurrentHair.Value = hair;
        // CurrentBeard.Value = beard + "_temp";
        CurrentBeard.Value = beard;
        // CurrentHelmet.Value = helmet + "_temp";
        CurrentHelmet.Value = helmet;
        // CurrentSkin.Value = skin + "_temp";
        CurrentSkin.Value = skin;
        UpdateOutfitsClientRpc();
    }

    [ClientRpc]
    private void UpdateOutfitsClientRpc()
    {
        UpdateOutfits();
    }

    private void UpdateOutfits()
    {
        ChangeSkin(CurrentSkin.Value.ToString());
        ChangeHair(CurrentHair.Value.ToString());
        ChangeBeard(CurrentBeard.Value.ToString());
        ChangeHelmet(CurrentHelmet.Value.ToString());
    }

    private void ChangeSkin(string partName)
    {
        foreach (Transform child in SkinContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

    private void ChangeHelmet(string partName)
    {
        foreach (Transform child in HelmetContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

    private void ChangeBeard(string partName)
    {
        foreach (Transform child in BeardContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

    private void ChangeHair(string partName)
    {
        foreach (Transform child in HairContentParent)
        {
            child.gameObject.SetActive(child.gameObject.name == partName);
        }
    }

}
