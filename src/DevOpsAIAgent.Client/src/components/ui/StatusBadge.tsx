import { Badge } from './Badge';
import { CICDEvent, Incident } from '../../types/api';

interface StatusBadgeProps {
  status: CICDEvent['status'] | Incident['status'] | Incident['severity'];
  className?: string;
}

export const StatusBadge = ({ status, className }: StatusBadgeProps) => {
  const getVariant = (status: string) => {
    switch (status) {
      case 'success':
      case 'resolved':
      case 'closed':
        return 'success';
      case 'failure':
      case 'error':
      case 'critical':
        return 'error';
      case 'pending':
      case 'investigating':
      case 'medium':
        return 'warning';
      case 'cancelled':
      case 'low':
        return 'secondary';
      case 'open':
      case 'high':
        return 'info';
      default:
        return 'default';
    }
  };

  const getDisplayText = (status: string) => {
    switch (status) {
      case 'success':
        return 'Success';
      case 'failure':
        return 'Failed';
      case 'pending':
        return 'Pending';
      case 'cancelled':
        return 'Cancelled';
      case 'open':
        return 'Open';
      case 'investigating':
        return 'Investigating';
      case 'resolved':
        return 'Resolved';
      case 'closed':
        return 'Closed';
      case 'low':
        return 'Low';
      case 'medium':
        return 'Medium';
      case 'high':
        return 'High';
      case 'critical':
        return 'Critical';
      default:
        return status.charAt(0).toUpperCase() + status.slice(1);
    }
  };

  return (
    <Badge variant={getVariant(status)} className={className}>
      {getDisplayText(status)}
    </Badge>
  );
};