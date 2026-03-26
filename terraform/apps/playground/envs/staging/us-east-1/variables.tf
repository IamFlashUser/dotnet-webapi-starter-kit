################################################################################
# General Variables
################################################################################

variable "environment" {
  type        = string
  description = "Environment name."
  default     = "staging"
}

variable "region" {
  type        = string
  description = "AWS region."
  default     = "us-east-1"
}

variable "domain_name" {
  type        = string
  description = "Domain name for the application (optional)."
  default     = null
}

################################################################################
# Network Variables
################################################################################

variable "vpc_cidr_block" {
  type        = string
  description = "CIDR block for the VPC."
}

variable "public_subnets" {
  description = "Public subnet definitions."
  type = map(object({
    cidr_block = string
    az         = string
  }))
}

variable "private_subnets" {
  description = "Private subnet definitions."
  type = map(object({
    cidr_block = string
    az         = string
  }))
}

variable "enable_nat_gateway" {
  type        = bool
  description = "Enable NAT Gateway."
  default     = true
}

variable "single_nat_gateway" {
  type        = bool
  description = "Use single NAT Gateway (cost savings)."
  default     = false
}

variable "enable_s3_endpoint" {
  type        = bool
  description = "Enable S3 VPC Gateway Endpoint."
  default     = true
}

variable "enable_ecr_endpoints" {
  type        = bool
  description = "Enable ECR VPC Interface Endpoints."
  default     = true
}

variable "enable_logs_endpoint" {
  type        = bool
  description = "Enable CloudWatch Logs VPC Interface Endpoint."
  default     = true
}

variable "enable_secretsmanager_endpoint" {
  type        = bool
  description = "Enable Secrets Manager VPC Interface Endpoint."
  default     = true
}

variable "enable_flow_logs" {
  type        = bool
  description = "Enable VPC Flow Logs."
  default     = true
}

################################################################################
# ECS Variables
################################################################################

variable "enable_container_insights" {
  type        = bool
  description = "Enable Container Insights."
  default     = true
}

################################################################################
# ALB Variables
################################################################################

variable "enable_https" {
  type        = bool
  description = "Enable HTTPS on ALB."
  default     = false
}

variable "acm_certificate_arn" {
  type        = string
  description = "ACM certificate ARN for HTTPS."
  default     = null
}

################################################################################
# S3 Variables
################################################################################

variable "app_s3_bucket_name" {
  type        = string
  description = "S3 bucket for application data."
}

variable "app_s3_enable_public_read" {
  type        = bool
  description = "Whether to enable public read on uploads prefix."
  default     = false
}

variable "app_s3_enable_cloudfront" {
  type        = bool
  description = "Whether to enable CloudFront in front of the app bucket."
  default     = true
}

variable "app_s3_cloudfront_price_class" {
  type        = string
  description = "Price class for CloudFront distribution."
  default     = "PriceClass_100"
}

################################################################################
# Database Variables
################################################################################

variable "db_name" {
  type        = string
  description = "Database name."
}

variable "db_username" {
  type        = string
  description = "Database admin username."
}

variable "db_password" {
  type        = string
  description = "Database admin password."
  sensitive   = true
  default     = null
}

variable "db_manage_master_user_password" {
  type        = bool
  description = "Let AWS manage the master user password in Secrets Manager."
  default     = true
}

variable "db_instance_class" {
  type        = string
  description = "RDS instance class."
  default     = "db.t4g.small"
}

variable "db_engine_version" {
  type        = string
  description = "PostgreSQL engine version."
  default     = "17"
}

variable "db_multi_az" {
  type        = bool
  description = "Enable Multi-AZ deployment."
  default     = false
}

variable "db_deletion_protection" {
  type        = bool
  description = "Enable deletion protection."
  default     = true
}

variable "db_enable_performance_insights" {
  type        = bool
  description = "Enable Performance Insights."
  default     = true
}

################################################################################
# Redis Variables
################################################################################

variable "redis_node_type" {
  type        = string
  description = "ElastiCache node type."
  default     = "cache.t4g.small"
}

variable "redis_num_cache_clusters" {
  type        = number
  description = "Number of cache clusters (nodes)."
  default     = 1
}

variable "redis_automatic_failover_enabled" {
  type        = bool
  description = "Enable automatic failover."
  default     = false
}

################################################################################
# WAF Variables
################################################################################

variable "enable_waf" {
  type        = bool
  description = "Enable AWS WAF for ALB protection."
  default     = true
}

variable "waf_rate_limit" {
  type        = number
  description = "Maximum requests per 5-minute period per IP."
  default     = 2000
}

variable "waf_enable_sqli_rule_set" {
  type        = bool
  description = "Enable SQL injection protection."
  default     = true
}

variable "waf_enable_ip_reputation_rule_set" {
  type        = bool
  description = "Enable IP reputation protection."
  default     = true
}

variable "waf_enable_anonymous_ip_rule_set" {
  type        = bool
  description = "Enable anonymous IP blocking."
  default     = false
}

variable "waf_enable_linux_rule_set" {
  type        = bool
  description = "Enable Linux OS protection rules."
  default     = true
}

variable "waf_enable_logging" {
  type        = bool
  description = "Enable WAF logging to CloudWatch."
  default     = true
}

################################################################################
# Container Image Variables
################################################################################

variable "container_registry" {
  type        = string
  description = "Container registry URL."
  default     = "ghcr.io/fullstackhero"
}

variable "container_image_tag" {
  type        = string
  description = "Container image tag (shared across all services)."
}

variable "api_image_name" {
  type        = string
  description = "API container image name."
  default     = "fsh-playground-api"
}

variable "blazor_image_name" {
  type        = string
  description = "Blazor container image name."
  default     = "fsh-playground-blazor"
}

################################################################################
# API Service Variables
################################################################################

variable "api_container_port" {
  type        = number
  description = "API container port."
  default     = 8080
}

variable "api_cpu" {
  type        = string
  description = "API CPU units."
  default     = "512"
}

variable "api_memory" {
  type        = string
  description = "API memory."
  default     = "1024"
}

variable "api_desired_count" {
  type        = number
  description = "Desired API task count."
  default     = 2
}

variable "api_enable_circuit_breaker" {
  type        = bool
  description = "Enable deployment circuit breaker."
  default     = true
}

variable "api_use_fargate_spot" {
  type        = bool
  description = "Use Fargate Spot capacity."
  default     = true
}

variable "api_enable_autoscaling" {
  type        = bool
  description = "Enable auto-scaling for API."
  default     = true
}

variable "api_autoscaling_min_capacity" {
  type        = number
  description = "Minimum API tasks when auto-scaling."
  default     = 2
}

variable "api_autoscaling_max_capacity" {
  type        = number
  description = "Maximum API tasks when auto-scaling."
  default     = 6
}

variable "api_autoscaling_cpu_target" {
  type        = number
  description = "Target CPU utilization for API auto-scaling."
  default     = 70
}

################################################################################
# Blazor Service Variables
################################################################################

variable "blazor_container_port" {
  type        = number
  description = "Blazor container port."
  default     = 8080
}

variable "blazor_cpu" {
  type        = string
  description = "Blazor CPU units."
  default     = "512"
}

variable "blazor_memory" {
  type        = string
  description = "Blazor memory."
  default     = "1024"
}

variable "blazor_desired_count" {
  type        = number
  description = "Desired Blazor task count."
  default     = 2
}

variable "blazor_enable_circuit_breaker" {
  type        = bool
  description = "Enable deployment circuit breaker."
  default     = true
}

variable "blazor_use_fargate_spot" {
  type        = bool
  description = "Use Fargate Spot capacity."
  default     = true
}

variable "blazor_enable_autoscaling" {
  type        = bool
  description = "Enable auto-scaling for Blazor."
  default     = true
}

variable "blazor_autoscaling_min_capacity" {
  type        = number
  description = "Minimum Blazor tasks when auto-scaling."
  default     = 2
}

variable "blazor_autoscaling_max_capacity" {
  type        = number
  description = "Maximum Blazor tasks when auto-scaling."
  default     = 6
}

variable "blazor_autoscaling_cpu_target" {
  type        = number
  description = "Target CPU utilization for Blazor auto-scaling."
  default     = 70
}
