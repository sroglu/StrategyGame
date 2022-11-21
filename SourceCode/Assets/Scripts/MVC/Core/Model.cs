using System;
using System.Collections.Generic;

namespace mehmetsrl.MVC.core
{
    public interface IModel : IDisposable
    {
        uint InstanceId(int subModelIndex = 0);
    }

    public enum ModelType
    {
        Single,
        Array
    }
    public enum ModelData
    {
        PhaseDone,
    }

    public abstract class ModelBase : IModel, ICloneable
    {
        public uint instanceId { get { return _instanceId; } }
        uint _instanceId = 0;
        public bool Initiated { get { return instanceId > 0; } }
        public ModelType modelType { get { return _modelType; } }
        ModelType _modelType = ModelType.Array;


        protected static Dictionary<uint, IModel> modelDictionary = new Dictionary<uint, IModel>();

        protected uint RegisterNewModel()
        {
            _instanceId = (uint)modelDictionary.Count + 1;
            modelDictionary.Add(_instanceId, this);
            _modelType = ModelType.Single;
            return _instanceId;
        }
        public static IModel GetModelByInstanceId(uint instanceId)
        {
            return modelDictionary[instanceId];
        }

        public static void ResetDictionary()
        {
            modelDictionary.Clear();
        }

        public virtual void Dispose()
        {
            UnregisterModel();
        }
        void UnregisterModel()
        {
            if (modelType == ModelType.Single)
            {
                if (modelDictionary.ContainsKey(instanceId))
                    modelDictionary.Remove(instanceId);
            }
        }

        public abstract uint InstanceId(int subModelIndex);
        public object Clone()
        {
            return MemberwiseClone();
        }

    }

    [System.Serializable]
    public abstract class Model<T> : ModelBase where T : ICloneable
    {
        public override uint InstanceId(int subModelIndex = 0)
        {
            if (subModels != null)
                return subModels[subModelIndex].InstanceId();
            return instanceId;
        }

        public static new Model<T> GetModelByInstanceId(uint instanceId)
        {
            return ModelBase.GetModelByInstanceId(instanceId) as Model<T>;
        }

        protected T DescriptionData { get; private set; }
        protected T[] DescriptionDataArr { get; private set; }
        public T CurrentData { get; protected set; }
        public T[] CurrentDataArr { get; protected set; }
        public Model<T>[] SubModels { get { return subModels; } }
        private Model<T>[] subModels;

        public Model(T data)
        {
            DescriptionData = data;
            UpdateCurrentData();
            RegisterNewModel();
        }

        public Model(T[] dataArr)
        {
            DescriptionDataArr = dataArr;
            UpdateCurrentData();
            CreateSubModels(out subModels);
        }

        #region Operations

        //public void UpdateModel(T data)
        //{
        //    DescriptionData = data;
        //    UpdateCurrentData();
        //}
        //public void UpdateModel(T[] dataArr)
        //{
        //    DescriptionDataArr = dataArr;
        //    UpdateCurrentData();
        //}

        public void Update(T data)
        {
            CurrentData = data;
            UpdateDescriptionData();
        }
        public void Update(T[] data)
        {
            CurrentDataArr = data;
            UpdateDescriptionData();
        }
        public void UpdateCurrentData()
        {
            if (DescriptionData != null)
                UpdateCurrentData(DescriptionData);

            if (DescriptionDataArr != null)
                UpdateCurrentData(DescriptionDataArr);
        }
        void UpdateCurrentData(T data)
        {
            CurrentData = (T)data.Clone();
        }
        void UpdateCurrentData(T[] dataArr)
        {
            Array.Copy(dataArr, CurrentDataArr, dataArr.Length);
        }

        public void UpdateDescriptionData()
        {
            if (CurrentData != null)
                UpdateDescriptionData(CurrentData);

            if (CurrentDataArr != null)
                UpdateDescriptionData(CurrentDataArr);
        }
        void UpdateDescriptionData(T data)
        {
            DescriptionData = (T)data.Clone();
        }
        void UpdateDescriptionData(T[] dataArr)
        {
            Array.Copy(dataArr, DescriptionDataArr, dataArr.Length);
        }
        #endregion


        protected virtual void CreateSubModels(out Model<T>[] subModels) { subModels = null; }
    }
}