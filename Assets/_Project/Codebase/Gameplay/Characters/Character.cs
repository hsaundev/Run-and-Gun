﻿using System.Threading.Tasks;
using _Project.Codebase.Gameplay.AI;
using _Project.Codebase.Gameplay.Player;
using _Project.Codebase.Gameplay.Projectiles;
using _Project.Codebase.Gameplay.World;
using _Project.Codebase.Modules;
using Cysharp.Threading.Tasks;
using DanonFramework.Runtime.Core.Utilities;
using UnityEngine;

namespace _Project.Codebase.Gameplay.Characters
{
    public class Character : IFloorObject, IProjectileHittable, IPlayerSelectable
    {
        public readonly NavmeshAgent agent;
        public readonly Transform transform;

        public Vector2Int FloorPos { get; set; }
        public int actionPoints = DEFAULT_MAX_ACTION_POINTS;
        public int moveDistancePerActionPoint = DEFAULT_MOVE_DISTANCE_PER_ACTION_POINT;

        public int MaxMaxActionPoints => DEFAULT_MAX_ACTION_POINTS;

        private CharacterRenderer m_renderer;
        
        protected const int DEFAULT_MAX_ACTION_POINTS = 1;
        protected const int DEFAULT_MOVE_DISTANCE_PER_ACTION_POINT = 6;

        public Character(Vector2Int position, NavmeshAgent agent, CharacterRenderer characterRenderer)  
        {
            this.agent = agent;
            m_renderer = characterRenderer;
            transform = agent.transform;
            agent.onReachPathEnd += OnReachPathEnd;
            UpdateFloorPosition(position, true);
        }
        
        public async UniTask PerformAction(CharacterAction action)
        {
            if (action != null)
            {
                actionPoints -= action.ActionPointCost;
                await action.OnStartAction();
                await action.Update();
                await action.OnEndAction();
            }
        }

        private void UpdateFloorPosition(Vector2Int gridPos, bool teleportToPos = false)
        {
            FloorPos = gridPos;
            Building building = ModuleUtilities.Get<GameModule>().Building;
            building.SetFloorObjectAtPos(gridPos, this);
            if (teleportToPos)
                transform.position = building.GridToWorld(FloorPos);
        }

        protected virtual void OnReachPathEnd(Vector2 worldPos, Vector2Int gridPos)
        {
            UpdateFloorPosition(gridPos, true);
        }

        public void OnProjectileHit() {}
        
        public void SetPlayerSelectState(bool state)
        {
            m_renderer.SelectionRenderer.SetSelectionState(state);
        }

        public virtual PlayerSelectableType SelectableType { get; set; }
    }
}