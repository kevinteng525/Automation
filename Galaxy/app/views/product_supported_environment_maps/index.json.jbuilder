json.array!(@product_supported_environment_maps) do |product_supported_environment_map|
  json.extract! product_supported_environment_map, :project_id, :supported_environment_id
  json.url product_supported_environment_map_url(product_supported_environment_map, format: :json)
end
