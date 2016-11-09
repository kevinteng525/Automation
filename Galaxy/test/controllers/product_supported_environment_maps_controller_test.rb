require 'test_helper'

class ProductSupportedEnvironmentMapsControllerTest < ActionController::TestCase
  setup do
    @product_supported_environment_map = product_supported_environment_maps(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:product_supported_environment_maps)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create product_supported_environment_map" do
    assert_difference('ProductSupportedEnvironmentMap.count') do
      post :create, product_supported_environment_map: { product_id: @product_supported_environment_map.product_id, supported_environment_id: @product_supported_environment_map.supported_environment_id }
    end

    assert_redirected_to product_supported_environment_map_path(assigns(:product_supported_environment_map))
  end

  test "should show product_supported_environment_map" do
    get :show, id: @product_supported_environment_map
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @product_supported_environment_map
    assert_response :success
  end

  test "should update product_supported_environment_map" do
    patch :update, id: @product_supported_environment_map, product_supported_environment_map: { product_id: @product_supported_environment_map.product_id, supported_environment_id: @product_supported_environment_map.supported_environment_id }
    assert_redirected_to product_supported_environment_map_path(assigns(:product_supported_environment_map))
  end

  test "should destroy product_supported_environment_map" do
    assert_difference('ProductSupportedEnvironmentMap.count', -1) do
      delete :destroy, id: @product_supported_environment_map
    end

    assert_redirected_to product_supported_environment_maps_path
  end
end
