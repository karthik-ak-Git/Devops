using DevOpsAIAgent.Core.Models;

namespace DevOpsAIAgent.Core.Messages;

public record PipelineFailedMessage(CiCdEvent Event);
