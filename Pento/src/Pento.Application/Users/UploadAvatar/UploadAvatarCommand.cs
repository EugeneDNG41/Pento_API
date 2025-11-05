using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Users.UploadAvatar;

public record class UploadAvatarCommand(IFormFile File) : ICommand<Uri>;
