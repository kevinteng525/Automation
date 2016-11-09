require 'test_helper'

class ProvidersControllerTest < ActionController::TestCase
  setup do
    @provider = providers(:one)
  end

  test "should get index" do
    get :index
    assert_response :success
    assert_not_nil assigns(:providers)
  end

  test "should get new" do
    get :new
    assert_response :success
  end

  test "should create provider" do
    assert_difference('Provider.count') do
      post :create, provider: { category: @provider.category, config: @provider.config, description: @provider.description, integer: @provider.integer, is_active: @provider.is_active, name: @provider.name, path: @provider.path, string: @provider.string, type: @provider.type }
    end

    assert_redirected_to provider_path(assigns(:provider))
  end

  test "should show provider" do
    get :show, id: @provider
    assert_response :success
  end

  test "should get edit" do
    get :edit, id: @provider
    assert_response :success
  end

  test "should update provider" do
    patch :update, id: @provider, provider: { category: @provider.category, config: @provider.config, description: @provider.description, integer: @provider.integer, is_active: @provider.is_active, name: @provider.name, path: @provider.path, string: @provider.string, type: @provider.type }
    assert_redirected_to provider_path(assigns(:provider))
  end

  test "should destroy provider" do
    assert_difference('Provider.count', -1) do
      delete :destroy, id: @provider
    end

    assert_redirected_to providers_path
  end
end
