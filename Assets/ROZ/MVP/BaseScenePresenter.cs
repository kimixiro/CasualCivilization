﻿using ROZ.MVP.Options;
using Assert = UnityEngine.Assertions.Assert;

namespace ROZ.MVP
{
    public abstract class BaseScenePresenter<TModel>
        : BasePresenter
        , IHasModel<TModel>
        where TModel : class
    {
        public TModel Model { get; private set; }

        public void SetModel(TModel model)
        {
            Assert.IsNotNull(model);
            Assert.IsNull(Model);

            Model = model;
        }
    }
}