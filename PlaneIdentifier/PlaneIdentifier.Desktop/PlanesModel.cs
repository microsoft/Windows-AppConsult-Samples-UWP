using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// bdbea542-afd3-4469-8652-b76d6442d9be_9b0c0651-fb87-440b-b67e-e2aa20fc1fef

namespace PlaneIdentifier.Desktop
{
    public sealed class PlanesModelInput
    {
        public VideoFrame data { get; set; }
    }

    public sealed class PlanesModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> loss { get; set; }
        public PlanesModelOutput()
        {
            this.classLabel = new List<string>();
            this.loss = new Dictionary<string, float>()
            {
                { "plane", float.NaN },
            };
        }
    }

    public sealed class PlanesModel
    {
        private LearningModelPreview learningModel;
        public static async Task<PlanesModel> CreatePlanesModel(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            PlanesModel model = new PlanesModel();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<PlanesModelOutput> EvaluateAsync(PlanesModelInput input) {
            PlanesModelOutput output = new PlanesModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("data", input.data);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("loss", output.loss);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
