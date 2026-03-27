################################################################################
# Environment Settings
################################################################################

environment = "staging"
region      = "us-east-1"

# Uncomment to enable HTTPS with a custom domain
# domain_name         = "staging.example.com"
# enable_https        = true
# acm_certificate_arn = "arn:aws:acm:us-east-1:ACCOUNT_ID:certificate/CERT_ID"

################################################################################
# Network Configuration
################################################################################

vpc_cidr_block = "10.20.0.0/16"

public_subnets = {
  a = {
    cidr_block = "10.20.0.0/24"
    az         = "us-east-1a"
  }
  b = {
    cidr_block = "10.20.1.0/24"
    az         = "us-east-1b"
  }
}

private_subnets = {
  a = {
    cidr_block = "10.20.10.0/24"
    az         = "us-east-1a"
  }
  b = {
    cidr_block = "10.20.11.0/24"
    az         = "us-east-1b"
  }
}

# Use single NAT gateway in staging to reduce costs
single_nat_gateway = true

# VPC Endpoints
enable_s3_endpoint             = true
enable_ecr_endpoints           = true
enable_logs_endpoint           = true
enable_secretsmanager_endpoint = true

# Enable flow logs for audit
enable_flow_logs = true

################################################################################
# WAF Configuration
################################################################################

enable_waf                        = true
waf_rate_limit                    = 2000
waf_enable_sqli_rule_set          = true
waf_enable_ip_reputation_rule_set = true
waf_enable_logging                = true

################################################################################
# S3 Configuration
################################################################################

app_s3_bucket_name        = "staging-fsh-app-bucket"
app_s3_enable_public_read = false
app_s3_enable_cloudfront  = true

################################################################################
# Database Configuration
################################################################################

db_name     = "fshdb"
db_username = "fshadmin"

# Use AWS Secrets Manager for password (recommended)
db_manage_master_user_password = true

# Staging uses Graviton instances
db_instance_class              = "db.t4g.small"
db_enable_performance_insights = true
db_deletion_protection         = true

################################################################################
# Redis Configuration (Graviton)
################################################################################

redis_node_type = "cache.t4g.small"

################################################################################
# Container Images
################################################################################

# Single tag for all container images
container_image_tag = "staging"

################################################################################
# Service Configuration
################################################################################

api_desired_count    = 2
api_use_fargate_spot = true

blazor_desired_count    = 2
blazor_use_fargate_spot = true

# Auto-scaling for staging
api_enable_autoscaling       = true
api_autoscaling_min_capacity = 2
api_autoscaling_max_capacity = 6
api_autoscaling_cpu_target   = 70

blazor_enable_autoscaling       = true
blazor_autoscaling_min_capacity = 2
blazor_autoscaling_max_capacity = 6
blazor_autoscaling_cpu_target   = 70

# Enable Container Insights for monitoring
enable_container_insights = true
