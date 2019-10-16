using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Loads map from level_.txt file, renders tiles, parses and applies rules.
 * IMPORTANT: tiles exist in a plane where Y axis grows downwards (like in images)
 */
public class Map : MonoBehaviour
{
    public int levelId;
    public int width;
    public int height;
    public List<Entity>[][] spots;
    private List<(EntityType, EntityType)> _prevRules;

    private ObjectFitContain _contain; 
    
    void Start()
    {
        string levelFile = $"Levels/{levelId}";
        TextAsset text = Resources.Load<TextAsset>(levelFile);
        string[] lines = text.text.Split('\n');

        width = int.Parse(lines[0]);
        height = int.Parse(lines[1]);

        spots = new List<Entity>[height][];

        for (int y = 0; y < height; y++)
        {
            spots[y] = new List<Entity>[width];
            for (int x = 0; x < width; x++)
            {
                spots[y][x] = new List<Entity>(2); // entities rarely stack in more than 2
                var id = int.Parse(lines[2].Substring((width * y + x) * 2, 2));
                
                // we skip id 0, which encodes an empty tile
                if (id == 0) continue;
                
                EntityType type;
                    
                try
                {
                    type = (EntityType) id;
                }
                catch (InvalidCastException)
                {
                    Debug.LogWarning($"Invalid entity type {id} in map file {levelFile}");
                    continue;
                }
                    
                var obj = new GameObject();
                obj.transform.parent = transform;
                var entity = obj.AddComponent<Entity>();
                entity.x = x;
                entity.y = y;
                entity.SetEntityType(type);
                spots[y][x].Add(entity);
            }
        }
        
        _prevRules = new List<(EntityType, EntityType)>();
        UpdateRules();

        _contain = gameObject.AddComponent<ObjectFitContain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_contain.Contain(width, height))
        {
            // invert Y transform
            var localPos = gameObject.transform.localPosition;
            gameObject.transform.localPosition = new Vector3(localPos.x, -localPos.y, localPos.z);
        }
    }

    List<(EntityType, EntityType)> ExtractRules()
    {
        var output = new List<(EntityType, EntityType)>();
        
        foreach (var row in spots)
        {
            foreach (var column in row)
            {
                foreach (Entity entity in column)
                {
                    if (entity.GetEntityType() == EntityType.ConnectorIs)
                    {
                        ExtractRulesFromConnectorIs(entity, output);
                    }
                }
            }
        }

        return output;
    }

    private void ExtractRulesFromConnectorIs(Entity entity, List<(EntityType, EntityType)> list)
    {
        // is connector can be a part of a vertical rule
        if (entity.y != 0 && entity.y != height - 1)
        {
            ExtractRulesFromSpots(spots[entity.y - 1][entity.x], spots[entity.y + 1][entity.x], list);
        }
        
        // is connector can be a part of a horizontal rule
        if (entity.x != 0 && entity.x != width - 1)
        {
            ExtractRulesFromSpots(spots[entity.y][entity.x - 1], spots[entity.y][entity.x + 1], list);
        }
    }

    private void ExtractRulesFromSpots(List<Entity> primarySpot, List<Entity> secondarySpot, List<(EntityType, EntityType)> list)
    {
        foreach (var primary in primarySpot)
        {
            var primaryType = primary.GetEntityType();
            var isPrimarySubject = Entity.IsSubject(primaryType);
            var isPrimaryTrait = Entity.IsTrait(primaryType);

            if (!isPrimarySubject && !isPrimaryTrait) continue;
                
            foreach (var secondary in secondarySpot)
            {
                var secondaryType = secondary.GetEntityType();
                var isSecondarySubject = Entity.IsSubject(secondaryType);
                var isSecondaryTrait = Entity.IsTrait(secondaryType);

                if (isPrimarySubject && isSecondaryTrait)
                {
                    list.Add((primaryType, secondaryType));
                }

                if (isSecondarySubject && isPrimaryTrait)
                {
                    list.Add((secondaryType, primaryType));
                }
            }
        }
    }

    public void UpdateRules()
    {
        var newRules = ExtractRules();
        var addedRules = newRules.Except(_prevRules);
        var deletedRules = _prevRules.Except(newRules);

        foreach (var (subject, trait) in deletedRules)
        {
            var targetType = Entity.GetSubjectType(subject);
            var entities = gameObject.GetComponentsInChildren<Entity>();

            foreach (var entity in entities)
            {
                if (entity.GetEntityType() != targetType) continue;
                
                var component = entity.gameObject.GetComponent(Entity.GetTraitBehavior(trait));
                if (component) Destroy(component);
            }
        }
        
        foreach (var (subject, trait) in addedRules)
        {
            var targetType = Entity.GetSubjectType(subject);
            var entities = gameObject.GetComponentsInChildren<Entity>();

            foreach (var entity in entities)
            {
                if (entity.GetEntityType() != targetType) continue;

                entity.gameObject.AddComponent(Entity.GetTraitBehavior(trait));
            }
        }

        _prevRules = newRules;
    }
}
