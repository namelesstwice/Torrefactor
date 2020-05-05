using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Torrefactor.Tests.Common
{
    public abstract class DockerContainer<TClient>
    {
        private string _containerId;

        protected abstract string Image { get; }
        protected abstract string Tag { get; }
        protected abstract string ContainerPort { get; }
        protected int HostPort { get; private set; }

        public async Task Run()
        {
            if (_containerId != null)
                throw new InvalidOperationException("Container is already running");

            using var client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
            var targetImage = await PullImage(client);
            var (container, hostPort) = await StartContainer(client, targetImage);
            (_containerId, HostPort) = (container.ID, hostPort);
            await CheckIfContainerRunning(client, container);
        }

        public async Task StopAndRemove()
        {
            if (_containerId == null)
                throw new InvalidOperationException("Container is not started");

            using var client = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
            await client.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());
            await client.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters());
            _containerId = null;
        }

        public TClient Connect()
        {
            if (_containerId == null)
                throw new InvalidOperationException("Container is not started");

            return CreateConnection();
        }

        private static async Task CheckIfContainerRunning(DockerClient client, CreateContainerResponse container)
        {
            var retryCount = 200;
            var containerState = await client.Containers.InspectContainerAsync(container.ID);
            while (!containerState.State.Running && retryCount-- > 0)
            {
                await Task.Delay(50);
                containerState = await client.Containers.InspectContainerAsync(container.ID);
            }
        }

        private async Task<(CreateContainerResponse, int)> StartContainer(DockerClient client,
            ImagesListResponse targetImage)
        {
            var hostPort = new Random((int) DateTime.UtcNow.Ticks).Next(10000, 12000);

            var container = await client.Containers.CreateContainerAsync(new CreateContainerParameters
            {
                ExposedPorts = new Dictionary<string, EmptyStruct>
                {
                    [ContainerPort] = new EmptyStruct()
                },
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                    {
                        [ContainerPort] = new List<PortBinding>
                            {new PortBinding {HostIP = "0.0.0.0", HostPort = $"{hostPort}"}}
                    }
                },
                Image = $"{Image}:{Tag}"
            });

            var isStarted = await client.Containers.StartContainerAsync(container.ID, new ContainerStartParameters
            {
                DetachKeys = $"d={targetImage}"
            });

            if (!isStarted) throw new Exception($"Could not start container: {container.ID}");

            return (container, hostPort);
        }

        private async Task<ImagesListResponse> PullImage(DockerClient client)
        {
            var image = await GetLocalImage(client);
            if (image != null)
                return image;

            await client.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = Image,
                    Tag = Tag
                },
                null,
                new Progress<JSONMessage>());

            return await GetLocalImage(client);
        }

        private async Task<ImagesListResponse> GetLocalImage(DockerClient client)
        {
            var images = await client.Images.ListImagesAsync(new ImagesListParameters
            {
                MatchName = $"{Image}:{Tag}"
            });

            return images.SingleOrDefault();
        }

        protected abstract TClient CreateConnection();
    }
}