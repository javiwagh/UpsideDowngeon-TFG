using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class GlowHighlight : MonoBehaviour
{
    Dictionary<Renderer, Material[]> glowMaterialDictionary = new Dictionary<Renderer, Material[]>();
    Dictionary<Renderer, Material[]> originalMaterialDictionary = new Dictionary<Renderer, Material[]>();
    Dictionary<UnityEngine.Color, Material> catchedGlowMaterials = new Dictionary<UnityEngine.Color, Material>();

    public Material glowMaterial;
    private bool isGlowing = false;

    private UnityEngine.Color yellowColor = UnityEngine.Color.yellow;
    private UnityEngine.Color redColor = UnityEngine.Color.red;
    private UnityEngine.Color originalGlowColor;
    private float originalGlowPower;

    private void Awake() {
        SetMaterialDictionaries();
        originalGlowColor = glowMaterial.GetColor("_GlowColor");
        originalGlowPower = glowMaterial.GetFloat("_GlowPower");
    }

    private void SetMaterialDictionaries() {
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>()) {
            Material[] originalMaterials = renderer.materials;
            originalMaterialDictionary.Add(renderer, originalMaterials);
            Material[] newMaterials = new Material[renderer.materials.Length];
            for (int i = 0; i < originalMaterials.Length; i++) {
                Material mat = null;
                if (catchedGlowMaterials.TryGetValue(originalMaterials[i].color, out mat) == false) {
                    mat = new Material (glowMaterial);
                    mat.color = originalMaterials[i].color;
                }
                newMaterials[i] = mat;
            }
            glowMaterialDictionary.Add(renderer, newMaterials);
        }
    }

    public void ToggleGlow() {
        if (isGlowing == false) {
            foreach (Renderer renderer in originalMaterialDictionary.Keys) {
                renderer.materials = glowMaterialDictionary[renderer];
            }
        }
        else {
            foreach(Renderer renderer in originalMaterialDictionary.Keys) {
                renderer.materials = originalMaterialDictionary[renderer];
            }
        }
        isGlowing = !isGlowing;
    }

    public void ToggleGlow(bool state) {
        if (isGlowing == state) return;
        isGlowing = !state;
        ToggleGlow();
    }

    public void ResetHighlight() {
        if (isGlowing == false) return;
        foreach(Renderer renderer in glowMaterialDictionary.Keys) {
            foreach(Material item in glowMaterialDictionary[renderer]) {
                item.SetColor("_GlowColor", originalGlowColor);
                item.SetFloat("_GlowPower", originalGlowPower);
            }
            renderer.materials = glowMaterialDictionary[renderer];
        }
    }

    public void HighlightPath() {
        if (isGlowing == false) return;
        foreach(Renderer renderer in glowMaterialDictionary.Keys) {
            foreach(Material item in glowMaterialDictionary[renderer]) {
                item.SetColor("_GlowColor", yellowColor);
                item.SetFloat("_GlowPower", 1.0f);
            }
            renderer.materials = glowMaterialDictionary[renderer];
        }
    }

    public void HighlightTarget() {
        if (isGlowing == false) return;
        foreach(Renderer renderer in glowMaterialDictionary.Keys) {
            foreach(Material item in glowMaterialDictionary[renderer]) {
                item.SetColor("_GlowColor", redColor);
                item.SetFloat("_GlowPower", 1.0f);
            }
            renderer.materials = glowMaterialDictionary[renderer];
        }
    }
}
