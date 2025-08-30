using System;
using UnityEngine;
using static BiomInfo;

public abstract class AbstractBiom
{
    protected EnvironmentSettings settings;
    protected string description;
    protected Action<Car> customEnterAction;
    protected Action<Car> customExitAction;

    protected AbstractBiom(EnvironmentSettings envSettings, string desc, Action<Car> enterAction = null, Action<Car> exitAction = null)
    {
        settings = envSettings;
        description = desc;
        customEnterAction = enterAction;
        customExitAction = exitAction;
    }

    public virtual void OnEnter(Car player)
    {
        ApplyPenalties(player);
        customEnterAction?.Invoke(player);
    }

    public virtual void OnExit(Car player)
    {
        RemovePenalties(player);
        customExitAction?.Invoke(player);
    }

    public void GenerateTerrain(TerrainGenerator generator)
    {
        generator.perlinNoiseFrequency += settings.perlinNoiseFrequency;
        generator.heightVariation += settings.heightVariation;
        generator.mountainThreshold += settings.mountainThreshold;
        generator.smoothing += settings.smoothing;
        generator.Texture = settings.terrainTexture;
        generator.environmentObjectsSettings = settings.environmentObjects;
        generator.coinSpacing += settings.coinSpacing;
        generator.fuelSpacing += settings.fuelSpacing;
        generator.addCoinSpacing += settings.addCoinSpacing;
        generator.addFuelSpacing += settings.addFuelSpacing;
    }

    public virtual void ApplyPenalties(Car player)
    {
        player._engineForce -= settings.speedPenalty;
        player._Expenditure += settings.fuelPenalty;
    }

    public virtual void RemovePenalties(Car player)
    {
        player._engineForce += settings.speedPenalty;
        player._Expenditure -= settings.fuelPenalty;
    }
    public static AbstractBiom CreateCityBiome(EnvironmentSettings settings)
    {
        return new GenericBiome(settings, "Urban environment with moderate terrain and penalties.",
            enterAction: player => {  },
            exitAction: player => {  });
    }

    public static AbstractBiom CreateDesertBiome(EnvironmentSettings settings)
    {
        return new GenericBiome(settings, "Arid environment with rough terrain and higher penalties.",
            enterAction: player => {  },
            exitAction: player => {  });
    }
    private class GenericBiome : AbstractBiom
    {
        public GenericBiome(EnvironmentSettings envSettings, string desc, Action<Car> enterAction, Action<Car> exitAction)
            : base(envSettings, desc, enterAction, exitAction)
        {
        }
    }
}